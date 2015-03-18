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

namespace MPM.Core.Dependency {
	public class Resolver : IResolver {
		public Configuration Resolve(Configuration target, Func<PackageSpec, NamedBuild[]> lookupPackageSpec) {
			var output = new List<PackageSpec>();
			//Packages which exist in the resultant configuration- Only one version of each package may exist in the result
			var includedPackages = new SortedSet<string>();

			foreach (var package in target.Packages) {
				if (package.Manual) {
					includedPackages.Add(package.Name);
					output.Add(package);
					continue;
				}

				var packageDetails = lookupPackageSpec(package).FirstOrDefault();
				if (packageDetails == null) {
					throw new DependencyException("Dependency could not be resolved because the package was not able to be looked up");
				}

				//foreach (var dependency in packageDetails.Dependencies) { }
			}
			return new Configuration {
				Packages = output.ToArray(),
			};
		}

		//I have no idea how to structure this part for conflict resolution to cooperate with later.
		public void ResolveDependency(PackageSpec package, Func<PackageSpec, NamedBuild[]> lookupPackageSpec, PackageSpec Constraints) {
			throw new NotImplementedException();
		}
	}
}
