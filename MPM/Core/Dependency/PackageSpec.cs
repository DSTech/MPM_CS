using System;
using semver.tools;

namespace MPM.Core.Dependency {
	public class PackageSpec : IEquatable<PackageSpec>, IComparable<PackageSpec> {
		public String Name { get; set; }
		public SemanticVersion Version { get; set; }
		public bool Manual { get; set; }

		public override bool Equals(object obj) {
			var packageSpec = obj as PackageSpec;
			if(packageSpec == null) {
				return false;
			}
			return Equals(packageSpec);
		}
		public override int GetHashCode() {
			return
				(Name?.GetHashCode() ?? 0)
				+ (Version?.GetHashCode() ?? 0)
				+ Manual.GetHashCode();
		}
		public bool Equals(PackageSpec other) {
			return
				Name == other.Name
				&& Version.Equals(other.Version)
				&& this.Manual == other.Manual;
		}
		public int CompareTo(PackageSpec other) {
			var byManual = Manual.CompareTo(other.Manual);
			if(byManual != 0) {
				return -byManual;
			}
			var byName = String.Compare(Name, other.Name);
			if(byName != 0) {
				return byName;
			}
			if(Version == null) {
				if(other.Version == null) {
					return 0;
				} else {
					return -1;
				}
			} else {
				if(other.Version == null) {
					return 1;
				}
			}
			var byVersion = Version.CompareTo(other.Version);
			if (byVersion != 0) {
				return byVersion;
			}
			return 0;
		}
		public static bool operator >(PackageSpec first, PackageSpec second) {
			return first.CompareTo(second) > 0;
		}
		public static bool operator <(PackageSpec first, PackageSpec second) {
			return first.CompareTo(second) < 0;
		}
	}
}
