using System;
using semver.tools;

namespace MPM.Types {
	public class InterfaceProvision : IEquatable<InterfaceProvision> {
		public InterfaceProvision() {
		}

		public InterfaceProvision(string interfaceName, SemanticVersion version) {
			this.InterfaceName = interfaceName;
			this.Version = version;
		}

		public String InterfaceName { get; set; }
		public SemanticVersion Version { get; set; }

		#region Equality members

		public bool Equals(InterfaceProvision other) {
			if (ReferenceEquals(null, other)) { return false; }
			if (ReferenceEquals(this, other)) { return true; }
			return string.Equals(this.InterfaceName, other.InterfaceName) && Equals(this.Version, other.Version);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) { return false; }
			if (ReferenceEquals(this, obj)) { return true; }
			var other = obj as InterfaceProvision;
			return other != null && Equals(other);
		}

		public override int GetHashCode() {
			unchecked {
				return ((this.InterfaceName != null ? this.InterfaceName.GetHashCode() : 0) * 397) ^
					(this.Version != null ? this.Version.GetHashCode() : 0);
			}
		}

		public static bool operator ==(InterfaceProvision left, InterfaceProvision right) {
			return Equals(left, right);
		}

		public static bool operator !=(InterfaceProvision left, InterfaceProvision right) {
			return !Equals(left, right);
		}

		#endregion
	}
}
