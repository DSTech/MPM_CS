using System;
using System.Collections.Generic;
using System.Linq;
using MPM.Core.Instances.Info;
using MPM.Core.Instances.Installation;
using MPM.Extensions;
using MPM.Types;

namespace MPM.Core.Dependency {

	public static class InstanceConfigurationExtensions {

		/// <summary>
		/// Checks for conflicts in the specified configuration.
		/// </summary>
		/// <param name="configuration"><see cref="InstanceConfiguration"/> within which to check for conflicts</param>
		/// <returns>An enumerable of tuples containing a specific <see cref="NamedBuild"/> and <see cref="Net.DTO.PackageConflict"/> specified by the returned build.</returns>
		public static IEnumerable<Tuple<Build, Conflict>> FindConflicts(this InstanceConfiguration configuration) {
			return from build in configuration.Packages
						 let conflicts = build.FindConflicts(
configuration
.Packages
.Where(spec => spec.PackageName != build.PackageName)
.ToArray()
)
						 from conflict in conflicts
						 select Tuple.Create(build, conflict);
		}

		/// <summary>
		/// Checks a specific <see cref="NamedBuild"/> for conflicts it may have with others in its configuration.
		/// </summary>
		/// <remarks>
		/// These interactions are one-way: other packages must check their conflicts with the source as well.
		/// </remarks>
		/// <param name="build">The build for which to check conflict conditions</param>
		/// <param name="otherBuilds">The other builds in the <see cref="InstanceConfiguration"/> to check conditions against</param>
		/// <returns>Any <see cref="Net.DTO.PackageConflict"/> triggered by a particular set of packages interacting with the given <see cref="NamedBuild"/></returns>
		public static IEnumerable<Conflict> FindConflicts(this Build build, Build[] otherBuilds) {
			var packageNames = otherBuilds
				.Select(b => b.PackageName)
				.ToArray();
			var interfaceNames = otherBuilds
				.SelectMany(b => b.InterfaceProvisions)
				.Select(b => b.InterfaceName)
				.ToArray();
			foreach (var conflict in build.Conflicts) {
				if (conflict.CheckPackageConflict(packageNames, interfaceNames)) {
					yield return conflict;
				}
			}
		}

		public static IFileMap GenerateFileMap(this InstanceConfiguration instanceConfiguration) {
			throw new NotImplementedException();
		}
	}
}
