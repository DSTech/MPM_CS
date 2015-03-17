using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.TopologicalSort;

namespace MPM.Core.Dependency {
	public class Resolver : IResolver {
		public Configuration Resolve(Configuration target) {
			//
			var output = new List<PackageSpec>();
			//Packages which exist in the resultant configuration- Only one version of each package may exist in the result
			var includedPackages = new SortedSet<string>();

			foreach (var package in target.Packages) {
				if(package.Manual) {
					includedPackages.Add(package.Name);
				}
			}
			throw new NotImplementedException();
			Debug.Assert(
				target
					.Packages
					.Where(p => p.Manual)
					.Except(output)
					.Count() == 0,
				"All manual packages must be accounted for in the resulting configuration"
			);
			Debug.Assert(
				output
					.Where(p => p.Manual)
					.Except(target.Packages)
					.Count() == 0,
				"All manual packages must be accounted for in the resulting configuration"
			);

			return new Configuration {
				Packages = output.ToArray(),
			};
		}
	}
}
