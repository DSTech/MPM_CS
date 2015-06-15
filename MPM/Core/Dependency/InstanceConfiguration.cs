using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Installation;
using MPM.Data;
using MPM.Net;
using MPM.Net.DTO;

namespace MPM.Core.Dependency {
	public static class InstanceConfigurationExtensions {
		/// <summary>
		/// Checks for conflicts in the specified configuration.
		/// </summary>
		/// <param name="configuration"><see cref="InstanceConfiguration"/> within which to check for conflicts</param>
		/// <returns>An enumerable of tuples containing a specific <see cref="NamedBuild"/> and <see cref="PackageConflict"/> specified by the returned build.</returns>
		public static IEnumerable<Tuple<NamedBuild, PackageConflict>> FindConflicts(this InstanceConfiguration configuration) {
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

		/// <summary>
		/// Checks a specific <see cref="NamedBuild"/> for conflicts it may have with others in its configuration.
		/// </summary>
		/// <remarks>
		/// These interactions are one-way: other packages must check their conflicts with the source as well.
		/// </remarks>
		/// <param name="namedBuild">The build for which to check conflict conditions</param>
		/// <param name="otherBuilds">The other builds in the <see cref="InstanceConfiguration"/> to check conditions against</param>
		/// <returns>Any <see cref="PackageConflict"/> triggered by a particular set of packages interacting with the given <see cref="NamedBuild"/></returns>
		public static IEnumerable<PackageConflict> FindConflicts(this NamedBuild namedBuild, NamedBuild[] otherBuilds) {
			var packageNames = otherBuilds
				.Select(b => b.Name)
				.ToArray();
			var interfaceNames = otherBuilds
				.SelectMany(b => b.InterfaceProvisions)
				.Select(b => b.Name)
				.ToArray();
			foreach (var conflict in namedBuild.Conflicts) {
				if (conflict.CheckPackageConflict(packageNames, interfaceNames)) {
					yield return conflict;
				}
			}
		}
		public static IFileMap GenerateFileMap(this InstanceConfiguration instanceConfiguration) {
			throw new NotImplementedException();
		}
	}
	public class InstanceConfiguration {
		public static InstanceConfiguration Empty { get; } = new InstanceConfiguration { Packages = new NamedBuild[0] };
		public NamedBuild[] Packages { get; set; }
	}
}
