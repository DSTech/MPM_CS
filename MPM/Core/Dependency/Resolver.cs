using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MPM.Net.DTO;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.TopologicalSort;
using QuickGraph.Data;
using QuickGraph.Collections;
using QuickGraph.Algorithms.Search;

namespace MPM.Core.Dependency {
	public class Resolver : IResolver {
		/// <summary>
		/// Sorts builds topologically, from least-dependent to most-dependent.
		/// </summary>
		/// <param name="builds">The builds to sort. All dependencies must be present or the input to ensure the proper ordering.</param>
		/// <returns>A sorted array of builds, with the least-dependent first</returns>
		public IEnumerable<NamedBuild> SortBuilds(IEnumerable<NamedBuild> builds) {
			var buildMap = builds.Distinct(b => b.Name).ToArray();

			var adjGraph = new AdjacencyGraph<int, Edge<int>>(false);
			adjGraph.AddVertexRange(Enumerable.Range(0, buildMap.Length));
			foreach (var buildIndex in Enumerable.Range(0, buildMap.Length)) {
				adjGraph.AddEdgeRange(
					buildMap
						.ElementAt(buildIndex)
						.Dependencies
						.Select(dep => Array.FindIndex<NamedBuild>(buildMap, nb => nb.Name == dep.Name))
						.Where(depIndex => depIndex != -1)
						.Select(destination => new Edge<int>(buildIndex, destination))
				);
			}
			var dfs = new DepthFirstSearchAlgorithm<int, Edge<int>>(adjGraph.ToArrayAdjacencyGraph());
			var onBackEdge = new EdgeAction<int, Edge<int>>(e => {
				var edge = (Edge<int>)e;
				adjGraph.RemoveEdge(edge);
			});
			try {
				dfs.BackEdge += onBackEdge;
				dfs.Compute();
			} finally {
				dfs.BackEdge -= onBackEdge;
			}
			return adjGraph
				.TopologicalSort()
				.Select(index => buildMap[index])
				.ToArray();
		}
		public ResolvedConfiguration Resolve(Configuration target, PackageSpecLookup lookupPackageSpec) {
			//Packages which exist in the resultant configuration- Only one version of a package may exist in the result
			var output = new List<NamedBuild>();

			foreach (var packageSpec in target.Packages) {
				if (!packageSpec.Manual) {
					continue;
				}

				NamedBuild[] resolvedSet;
				try {
					resolvedSet = ResolveRecursive(packageSpec, lookupPackageSpec);
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
			Debug.Assert(output.Count == 0 || output.Count == output.Distinct(package => package.Name).Count());
			return new ResolvedConfiguration {
				Packages = output.ToArray(),
			};
		}

		public NamedBuild ResolveDependency(PackageSpec packageSpec, PackageSpecLookup lookupPackageSpec, IEnumerable<DependencyConstraint> constraints = null, ResolutionMode resolutionMode = ResolutionMode.Highest) {
			var constraintsArr = constraints?.ToArray() ?? new DependencyConstraint[0];
			var namedBuilds = lookupPackageSpec(packageSpec);
			NamedBuild result;
			switch (resolutionMode) {
				case ResolutionMode.Highest:
					result = namedBuilds.FirstOrDefault(nb => constraintsArr.All(constraint => constraint.Allows(nb)));
					break;
				case ResolutionMode.HighestStable:
					NamedBuild highest = null;
					foreach (var build in namedBuilds) {
						if (!constraintsArr.All(constraint => constraint.Allows(build))) {
							continue;
						}
						if (build.Stable) {
							result = build;
							break;
						}
						if (highest == null) {
							highest = build;
						}
					}
					result = highest;
					break;
				default:
					throw new NotImplementedException($"{nameof(ResolutionMode)} \"{resolutionMode}\" is unsupported by {this.GetType().Name}");
			}
			if (result == null) {
				throw new DependencyException("Could not resolve package, no matching packages found.", packageSpec);
			}
			return result;
		}

		public NamedBuild[] ResolveRecursive(PackageSpec packageSpec, PackageSpecLookup lookupPackageSpec, IEnumerable<DependencyConstraint> constraints = null, ResolutionMode resolutionMode = ResolutionMode.Highest) {
			var output = new List<NamedBuild>();
			var resolvedBuild = ResolveDependency(packageSpec, lookupPackageSpec, constraints, resolutionMode);
			Debug.Assert(resolvedBuild != null, "ResolveDependency is not allowed to return null");
			output.Add(resolvedBuild);
			foreach (var dependency in resolvedBuild.Dependencies) {
				var depSpec = dependency.ToSpec();
				NamedBuild[] resolvedDeps;
				try {
					resolvedDeps = ResolveRecursive(depSpec, lookupPackageSpec, constraints, resolutionMode);
					Debug.Assert(resolvedDeps != null, "Recursive resolution may not return null");
				} catch (DependencyException e) {
					throw new DependencyException("Could not resolve package, no matching dependency tree found", e, depSpec, resolvedBuild);
				}
				output.AddRange(resolvedDeps);
			}
			return SortBuilds(output).ToArray();
		}
	}
}
