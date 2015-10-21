using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Extensions;
using MPM.Types;

namespace MPM.Core.Dependency {

	public static class ConfigurationExtensions {

		//Returns conflicts caused by particular builds
		//LookupBuild should return a package with exactly one build
		public static IEnumerable<Tuple<PackageSpec, Build, Conflict>> FindConflicts(this Configuration configuration, Func<PackageSpec, Package> lookupBuild) {
			foreach (var package in configuration.Packages) {
				var packageDetails = lookupBuild?.Invoke(package);
				if (packageDetails == null) {
					throw new Exception("Package details could not be found");
				}
				Debug.Assert(packageDetails.Builds != null && packageDetails.Builds.Count == 1);
				var build = packageDetails.Builds.First();

				var conflicts = build.FindConflicts(
					configuration
						.Packages
						.Where(spec => spec.Name != packageDetails.Name)
						.ToArray(),
					lookupBuild
				);
				foreach (var conflict in conflicts) {
					yield return Tuple.Create(package, build, conflict);
				}
			}
		}

		//Returns any conflicts triggered by a particular set of packages interacting with the given package
		//LookupBuild should return a package with exactly one build
		//These interactions are one-way: other packages must check their conflicts with the source as well
		public static IEnumerable<Conflict> FindConflicts(this Build build, PackageSpec[] otherPackageSpecs, Func<PackageSpec, Package> lookupBuild) {
			var packages = otherPackageSpecs
				.Select(spec => {
					var packageDetails = lookupBuild?.Invoke(spec);
					Debug.Assert(packageDetails.Builds != null && packageDetails.Builds.Count == 1);
					return packageDetails;
				})
				.ToArray();
			var packageNames = packages
				.Select(b => b.Name)
				.ToArray();
			var interfaceNames = packages
				.FirstOrDefault()
				.Builds
				.SelectMany(b => b.InterfaceProvisions)
				.Select(b => b.InterfaceName)
				.ToArray();
			foreach (var conflict in build.Conflicts) {
				if (conflict.CheckPackageConflict(packageNames, interfaceNames)) {
					yield return conflict;
				}
			}
		}
	}

	public class Configuration {

		public Configuration(IEnumerable<PackageSpec> packageSpecifications, CompatibilitySide side = CompatibilitySide.Universal) {
			this.Packages = packageSpecifications.ToArray();
			this.Side = side;
		}

		public IReadOnlyCollection<PackageSpec> Packages { get; }
		public CompatibilitySide Side { get; }

		public static Configuration Empty { get; } = new Configuration(Enumerable.Empty<PackageSpec>());
	}
}
