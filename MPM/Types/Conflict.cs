using System;

namespace MPM.Types {
    public class Conflict : IEquatable<Conflict> {
        public Conflict() {
        }

        public Conflict(ConflictCondition condition, ConflictResolution resolution) {
            this.Condition = condition;
            this.Resolution = resolution;
        }

        public ConflictCondition Condition { get; set; }
        public ConflictResolution Resolution { get; set; }

        #region Equality members

        public bool Equals(Conflict other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return Equals(this.Condition, other.Condition) && Equals(this.Resolution, other.Resolution);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            var other = obj as Conflict;
            return other != null && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                return ((this.Condition != null ? this.Condition.GetHashCode() : 0) * 397) ^
                    (this.Resolution != null ? this.Resolution.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Conflict left, Conflict right) {
            return Equals(left, right);
        }

        public static bool operator !=(Conflict left, Conflict right) {
            return !Equals(left, right);
        }

        #endregion
    }
}
