using System;

namespace MPM.Types {
    public class InterfaceDependency : IEquatable<InterfaceDependency> {
        public InterfaceDependency() {
        }

        public InterfaceDependency(string interfaceName, SemVer.Range versionSpec, CompatibilitySide side) {
            this.InterfaceName = interfaceName;
            this.VersionSpec = versionSpec;
            this.Side = side;
        }

        public String InterfaceName { get; set; }
        public SemVer.Range @VersionSpec { get; set; }
        public CompatibilitySide Side { get; set; }

        #region Equality members

        public bool Equals(InterfaceDependency other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return string.Equals(this.InterfaceName, other.InterfaceName) && Equals(this.VersionSpec, other.VersionSpec) &&
                this.Side == other.Side;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            var other = obj as InterfaceDependency;
            return other != null && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = this.InterfaceName != null ? this.InterfaceName.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (this.VersionSpec != null ? this.VersionSpec.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) this.Side;
                return hashCode;
            }
        }

        public static bool operator ==(InterfaceDependency left, InterfaceDependency right) {
            return Equals(left, right);
        }

        public static bool operator !=(InterfaceDependency left, InterfaceDependency right) {
            return !Equals(left, right);
        }

        #endregion
    }
}
