using System;

namespace MPM.Types {
    public class PackageDependency : IEquatable<PackageDependency> {
        public PackageDependency(string packageName, SemVer.Range versionSpec, CompatibilitySide side) {
            this.PackageName = packageName;
            this.VersionSpec = versionSpec;
            this.Side = side;
        }

        public String PackageName { get; set; }
        public SemVer.Range @VersionSpec { get; set; }
        public CompatibilitySide Side { get; set; }

        #region Equality members

        public bool Equals(PackageDependency other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return string.Equals(this.PackageName, other.PackageName) && Equals(this.VersionSpec, other.VersionSpec) &&
                this.Side == other.Side;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            var other = obj as PackageDependency;
            return other != null && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = this.PackageName != null ? this.PackageName.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (this.VersionSpec != null ? this.VersionSpec.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) this.Side;
                return hashCode;
            }
        }

        public static bool operator ==(PackageDependency left, PackageDependency right) {
            return Equals(left, right);
        }

        public static bool operator !=(PackageDependency left, PackageDependency right) {
            return !Equals(left, right);
        }

        #endregion
    }
}
