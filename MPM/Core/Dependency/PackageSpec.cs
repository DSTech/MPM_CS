using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MPM.Core.Instances.Info;
using MPM.Data;
using MPM.Types;
using NServiceKit.Common;
using semver.tools;

namespace MPM.Core.Dependency {

	public static class PackageSpecExtensions {

		public static PackageSpec ToSpec(this PackageDependency dependency, Arch arch, CompatibilityPlatform platform, bool manual = false) {
			return new PackageSpec {
				Manual = manual,
				Arch = arch,
				Platform = platform,
			}.PopulateWithNonDefaultValues(dependency);
		}

		/// <summary>
		/// Checks if a build satisfies the given specification.
		/// </summary>
		/// <param name="spec">Specification to check against</param>
		/// <param name="build">Build to check. Assumes (What?)</param>
		/// <returns></returns>
		public static bool Satisfies(this PackageSpec spec, Build build) {
			return spec.Name == build.PackageName
				&& spec.Arch == build.Arch
				&& spec.Platform == build.Platform
				&& (
					spec.Side == build.Side || build.Side == CompatibilitySide.Universal
				)
				&& spec.VersionSpec.Satisfies(build.Version);
		}

		/// <summary>
		/// Looks up a package, returning named builds qualifying for the specification.
		/// Must return in descending order of version.
		/// </summary>
		/// <param name="packageSpec">Specification to look up</param>
		/// <returns>Builds in descending order of version</returns>
		/// <remarks>Should be converted to return IQueryable to allow optimized behavior with constraint lookup</remarks>
		public static async Task<IEnumerable<Build>> LookupSpec(this IPackageRepository repository, PackageSpec packageSpec) {
			var package = await repository.FetchBuilds(packageSpec.Name, packageSpec.VersionSpec);
			return package
				.Where(b => packageSpec.Satisfies(b));
		}
	}

	public class PackageSpec : IEquatable<PackageSpec> {
		public String Name { get; set; }
		public Arch Arch { get; set; }
		public CompatibilityPlatform Platform { get; set; }
		public VersionSpec @VersionSpec { get; set; }
		public CompatibilitySide Side { get; set; } = CompatibilitySide.Universal;
		public bool Manual { get; set; } = false;

		public override bool Equals(object obj) {
			var packageSpec = obj as PackageSpec;
			if (packageSpec == null) {
				return obj == null;
			}
			return Equals(packageSpec);
		}

		public override int GetHashCode() {
			return
				(Name?.GetHashCode() ?? 0)
				+ (Arch?.GetHashCode() ?? 0)
				+ Platform.GetHashCode()
				+ (VersionSpec != null ? GetVersionSpecHashCode(VersionSpec) : 0)
				+ Manual.GetHashCode();
		}

		private int GetVersionSpecHashCode(VersionSpec spec) {
			return
				unchecked(
					spec.IsMinInclusive.GetHashCode() +
					spec.IsMaxInclusive.GetHashCode() +
					spec.MinVersion.GetHashCode() +
					spec.MaxVersion.GetHashCode());
		}

		private bool VersionSpecsEqual(VersionSpec first, VersionSpec second) {
			return
				first.IsMinInclusive == second.IsMinInclusive &&
				first.IsMaxInclusive == second.IsMaxInclusive &&
				first.MinVersion.Equals(second.MinVersion) &&
				first.MaxVersion.Equals(second.MaxVersion);
		}

		public bool Equals(PackageSpec other) {
			return
				Name == other.Name
				&& this.Arch == other.Arch
				&& this.Platform == other.Platform
				&& this.VersionSpec.ToString() == other.VersionSpec.ToString()
				&& this.Manual == other.Manual;
		}
	}
}
