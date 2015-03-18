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

namespace MPM.Core.Dependency {
	public class Resolver : IResolver {
		/// <summary>
		/// Sorts builds topologically, from least-dependent to most-dependent.
		/// </summary>
		/// <param name="builds">The builds to sort. All dependencies must be present or the input to ensure the proper ordering.</param>
		/// <returns>A sorted array of builds, with the least-dependent first</returns>
		public NamedBuild[] SortBuilds(NamedBuild[] builds) {
			var buildMap = builds.Distinct(b => b.Name).ToArray();

			var adjGraph = new AdjacencyGraph<int, Edge<int>>(false, builds.Length);
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

				var resolvedSet = ResolveRecursive(packageSpec, lookupPackageSpec);
				output.AddRange(resolvedSet.Where(elem => !output.Contains(elem)));
			}
			{
				var unsortedOutput = output.ToArray();
				output.Clear();
				output.AddRange(TopoSortBuilds(unsortedOutput));
			}
			Debug.Assert(output.Count == 0 || output.Count == output.Distinct(package => package.Name).Count());
			return new ResolvedConfiguration {
				Packages = output.ToArray(),
			};
		}

		public NamedBuild ResolveDependency(PackageSpec packageSpec, PackageSpecLookup lookupPackageSpec, IEnumerable<DependencyConstraint> constraints = null, ResolutionMode resolutionMode = ResolutionMode.Highest) {
			var constraintsArr = constraints?.ToArray() ?? new DependencyConstraint[0];
			var namedBuilds = lookupPackageSpec(packageSpec);
			switch (resolutionMode) {
				case ResolutionMode.Highest:
					return namedBuilds.FirstOrDefault(nb => constraintsArr.All(constraint => constraint.Allows(nb)));
				case ResolutionMode.HighestStable:
					NamedBuild highest = null;
					foreach (var build in namedBuilds) {
						if (!constraintsArr.All(constraint => constraint.Allows(build))) {
							continue;
						}
						if (build.Stable) {
							return build;
						}
						if (highest == null) {
							highest = build;
						}
					}
					return highest;
				default:
					throw new NotImplementedException($"{nameof(ResolutionMode)} \"{resolutionMode}\" is unsupported by {this.GetType().Name}");
			}
		}

		public NamedBuild[] ResolveRecursive(PackageSpec packageSpec, PackageSpecLookup lookupPackageSpec, IEnumerable<DependencyConstraint> constraints = null, ResolutionMode resolutionMode = ResolutionMode.Highest) {
			throw new NotImplementedException();
		}
	}
}
