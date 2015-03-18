using System;
using semver.tools;

namespace MPM.Core.Dependency {
	public class PackageSpec : IEquatable<PackageSpec> {
		public String Name { get; set; }
		public VersionSpec Version { get; set; }
		public bool Manual { get; set; }

		public override bool Equals(object obj) {
			var packageSpec = obj as PackageSpec;
			if(packageSpec == null) {
				return obj == null;
			}
			return Equals(packageSpec);
		}
		public override int GetHashCode() {
			return
				(Name?.GetHashCode() ?? 0)
				+ (Version != null ? GetVersionSpecHashCode(Version) : 0)
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
				&& Version.ToString() == other.Version.ToString()
				&& this.Manual == other.Manual;
		}
	}
}
