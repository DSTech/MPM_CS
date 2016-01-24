using System;
using System.Collections.Generic;
using System.Linq;

namespace MPM.Types {
    public class ConflictCondition : IEquatable<ConflictCondition> {
        public ConflictCondition() {
        }

        public ConflictCondition(
            string packageName = null,
            string interfaceName = null,
            IEnumerable<ConflictCondition> and = null,
            IEnumerable<ConflictCondition> or = null) {
            this.PackageName = packageName;
            this.InterfaceName = interfaceName;
            this.And = (and ?? Enumerable.Empty<ConflictCondition>()).ToList();
            this.Or = (or ?? Enumerable.Empty<ConflictCondition>()).ToList();
        }

        public List<ConflictCondition> And { get; set; }
        public List<ConflictCondition> Or { get; set; }
        public String InterfaceName { get; set; }
        public String PackageName { get; set; }

        #region Equality members

        public bool Equals(ConflictCondition other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return Equals(this.And, other.And) && Equals(this.Or, other.Or) &&
                string.Equals(this.InterfaceName, other.InterfaceName) && string.Equals(this.PackageName, other.PackageName);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            var other = obj as ConflictCondition;
            return other != null && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = this.And != null ? this.And.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (this.Or != null ? this.Or.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.InterfaceName != null ? this.InterfaceName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.PackageName != null ? this.PackageName.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ConflictCondition left, ConflictCondition right) {
            return Equals(left, right);
        }

        public static bool operator !=(ConflictCondition left, ConflictCondition right) {
            return !Equals(left, right);
        }

        #endregion
    }
}
