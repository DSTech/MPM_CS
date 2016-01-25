using System;
using System.Collections.Generic;

namespace MPM.Types {
    public struct BuildDependencySet : IEquatable<BuildDependencySet> {
        #region Equality members

        public bool Equals(BuildDependencySet other) {
            return Equals(this.Interfaces, other.Interfaces) && Equals(this.Packages, other.Packages);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            return obj is BuildDependencySet && this.Equals((BuildDependencySet) obj);
        }

        public override int GetHashCode() {
            unchecked { return ((this.Interfaces != null ? this.Interfaces.GetHashCode() : 0) * 397) ^ (this.Packages != null ? this.Packages.GetHashCode() : 0); }
        }

        public static bool operator ==(BuildDependencySet left, BuildDependencySet right) {
            return left.Equals(right);
        }

        public static bool operator !=(BuildDependencySet left, BuildDependencySet right) {
            return !left.Equals(right);
        }

        #endregion

        public List<InterfaceDependency> Interfaces { get; set; }

        public List<PackageDependency> Packages { get; set; }
    }
}