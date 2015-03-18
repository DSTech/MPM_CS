using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Net;
using MPM.Net.DTO;

namespace MPM.Core.Dependency {
	public static class ResolvedConfigurationExtensions {
		//Returns conflicts caused by particular builds
		//LookupBuild should return a package with exactly one build
		public static IEnumerable<Tuple<NamedBuild, PackageConflict>> FindConflicts(this ResolvedConfiguration configuration, PackageSpecLookup lookupBuild) {
			foreach (var build in configuration.Packages) {

				var conflicts = build.FindConflicts(
					configuration
						.Packages
						.Where(spec => spec.Name != build.Name)
						.ToArray()
				);
				foreach (var conflict in conflicts) {
					yield return Tuple.Create(build, conflict);
				}
			}
		}

		//Returns any conflicts triggered by a particular set of packages interacting with the given package
		//LookupBuild should return a package with exactly one build
		//These interactions are one-way: other packages must check their conflicts with the source as well
		public static IEnumerable<PackageConflict> FindConflicts(this NamedBuild build, NamedBuild[] otherBuilds) {
			var packageNames = otherBuilds
				.Select(b => b.Name)
				.ToArray();
			var interfaceNames = otherBuilds
				.SelectMany(b => b.InterfaceProvisions)
				.Select(b => b.Name)
				.ToArray();
			foreach (var conflict in build.Conflicts) {
				if (conflict.CheckPackageConflict(packageNames, interfaceNames)) {
					yield return conflict;
				}
			}
		}
	}
	public class ResolvedConfiguration {
		public static ResolvedConfiguration Empty { get; }
		= new ResolvedConfiguration {
			Packages = new NamedBuild[0],
		};
		public NamedBuild[] Packages { get; set; }
	}
}
