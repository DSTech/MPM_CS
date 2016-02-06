using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Info;
using MPM.Data;
using MPM.Data.Repository;
using MPM.Extensions;
using MPM.Types;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.Search;
using QuickGraph.Algorithms.TopologicalSort;
using QuickGraph.Collections;
using QuickGraph.Data;

namespace MPM.Core.Dependency {
    public class DependencyResolver : IDependencyResolver {
        public InstanceConfiguration Resolve(Configuration target, IPackageRepository repository) {
            //Packages which exist in the resultant configuration- Only one version of a package may exist in the result
            var output = new List<Build>();

            foreach (var packageSpec in target.Packages) {
                if (!packageSpec.Manual) {
                    continue;
                }

                IReadOnlyCollection<Build> resolvedSet;
                try {
                    resolvedSet = ResolveRecursive(packageSpec, repository);
                    Debug.Assert(resolvedSet != null);
                } catch (DependencyException e) {
                    throw new DependencyException("The specified dependencies could not be resolved", e);
                }
                output.AddRange(resolvedSet.Where(elem => !output.Contains(elem)));
            }
            {
                var unsortedOutput = output.ToArray();
                output.Clear();
                output.AddRange(SortBuilds(unsortedOutput));
            }
            Debug.Assert(output.Count == 0 || (output.Count == output.Distinct(b => b.PackageName).Count() && output.Select(o => o.PackageName).ToHashSet().SetEquals(output.Select(b => b.PackageName).Distinct().ToHashSet())));
            return new InstanceConfiguration(output);
        }

        public IReadOnlyCollection<Build> ResolveDependency(PackageSpec packageSpec, IPackageRepository repository, CompatibilitySide packageSide = CompatibilitySide.Universal, IEnumerable<DependencyConstraint> constraints = null) {
            var constraintsArr = constraints?.ToArray() ?? new DependencyConstraint[0];
            var namedBuilds = repository.LookupSpec(packageSpec).ToArray();
            var result = namedBuilds
                .Where(nb => (nb.Side == packageSide || nb.Side == CompatibilitySide.Universal) && constraintsArr.All(constraint => constraint.Allows(nb)))
                .OrderByDescending(nb => nb.Version)
                .ThenByDescending(nb => nb.Side == packageSide)
                .ToArray();
            if (result.Length == 0) {
                throw new DependencyException("Could not resolve package, no matching packages found.", packageSpec);
            }
            return result;
        }

        public IReadOnlyCollection<Build> ResolveRecursive(PackageSpec packageSpec, IPackageRepository repository, CompatibilitySide packageSide = CompatibilitySide.Universal, IEnumerable<DependencyConstraint> constraints = null) {
            var dependencyConstraints = constraints as DependencyConstraint[] ?? constraints?.ToArray() ?? new DependencyConstraint[0];
            var possibleBuilds = ResolveDependency(packageSpec, repository, packageSide, dependencyConstraints);
            Debug.Assert(possibleBuilds != null, "ResolveDependency is not allowed to return null");
            foreach (var possibleBuild in possibleBuilds) {
                var output = new List<Build>();
                Debug.Assert(possibleBuild != null, "ResolveDependency must not return null elements");
                output.Add(possibleBuild);
                foreach (var dependency in possibleBuild.Dependencies.Packages) {
                    var depSpec = dependency.ToSpec(packageSpec.Arch);
                    if (packageSide != CompatibilitySide.Universal && dependency.Side != CompatibilitySide.Universal && dependency.Side != packageSide) {
                        continue;
                    }
                    IReadOnlyCollection<Build> resolvedDeps;
                    try {
                        resolvedDeps = ResolveRecursive(depSpec, repository, packageSide, dependencyConstraints);
                        Debug.Assert(resolvedDeps != null, "Recursive resolution may not return null");
                    } catch (DependencyException) {
                        goto nextBuild;
                    }
                    output.AddRange(resolvedDeps);
                }
                return SortBuilds(output).ToArray();
                nextBuild:
                continue;
            }
            throw new DependencyException("Could not resolve package, no viable dependency tree found", packageSpec);
        }

        /// <summary>
        ///     Sorts builds topologically, from least-dependent to most-dependent.
        /// </summary>
        /// <param name="builds">The builds to sort. All dependencies must be present or the input to ensure the proper ordering.</param>
        /// <returns>A sorted array of builds, with the least-dependent first</returns>
        public static IEnumerable<Build> SortBuilds(IEnumerable<Build> builds) {
            var buildMap = builds.Distinct(b => b.PackageName).ToArray();

            var adjGraph = new AdjacencyGraph<int, Edge<int>>(false);
            adjGraph.AddVertexRange(Enumerable.Range(0, buildMap.Length));
            foreach (var buildIndex in Enumerable.Range(0, buildMap.Length)) {
                adjGraph.AddEdgeRange(
                    buildMap
                        .ElementAt(buildIndex)
                        .Dependencies.Packages
                        .Select(dep => Array.FindIndex<Build>(buildMap, nb => nb.PackageName == dep.PackageName))
                        .Where(depIndex => depIndex != -1)
                        .Select(destination => new Edge<int>(buildIndex, destination))
                    );
            }
            var dfs = new DepthFirstSearchAlgorithm<int, Edge<int>>(adjGraph.ToArrayAdjacencyGraph());
            var removeBackEdge = new EdgeAction<int, Edge<int>>(edge => {
                adjGraph.RemoveEdge(edge);
            });
            try {
                dfs.BackEdge += removeBackEdge;
                dfs.Compute();
            } finally {
                dfs.BackEdge -= removeBackEdge;
            }

            var results = adjGraph
                .TopologicalSort()
                .Reverse()
                .Select(index => buildMap[index])
                .ToArray();
            return results;
        }
    }
}
