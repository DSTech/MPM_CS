using System;
using System.Collections.Generic;
using System.Linq;

namespace MPM.Types {
    public class ForcedDependencySet : IEquatable<ForcedDependencySet> {
        public ForcedDependencySet() {
        }

        public ForcedDependencySet(IEnumerable<string> packageNames, IEnumerable<string> interfaceNames) {
            this.PackageNames = packageNames.ToList();
            this.InterfaceNames = interfaceNames.ToList();
        }

        public List<String> PackageNames { get; set; }
        public List<String> InterfaceNames { get; set; }

        #region Equality members

        public bool Equals(ForcedDependencySet other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return Equals(this.PackageNames, other.PackageNames) && Equals(this.InterfaceNames, other.InterfaceNames);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            var other = obj as ForcedDependencySet;
            return other != null && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                return ((this.PackageNames != null ? this.PackageNames.GetHashCode() : 0) * 397) ^
                    (this.InterfaceNames != null ? this.InterfaceNames.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ForcedDependencySet left, ForcedDependencySet right) {
            return Equals(left, right);
        }

        public static bool operator !=(ForcedDependencySet left, ForcedDependencySet right) {
            return !Equals(left, right);
        }

        #endregion
    }
}
