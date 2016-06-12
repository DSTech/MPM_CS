using System;
using Newtonsoft.Json;

namespace MPM.Types {
    public class DependencyConflictResolution : IEquatable<DependencyConflictResolution> {
        public DependencyConflictResolution() {
        }

        public DependencyConflictResolution(ForcedDependencySet force = null, DeclinedDependencySet decline = null) {
            this.Force = force;
            this.Decline = decline;
        }

        [JsonProperty(PropertyName = "Force", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ForcedDependencySet Force { get; set; }

        [JsonProperty(PropertyName = "Decline", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DeclinedDependencySet Decline { get; set; }

        public bool ShouldSerialize() => ShouldSerializeForce() || ShouldSerializeDecline();

        public bool ShouldSerializeForce() => Force?.ShouldSerialize() == true;

        public bool ShouldSerializeDecline() => Decline?.ShouldSerialize() == true;

        #region Equality members

        public bool Equals(DependencyConflictResolution other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return Equals(this.Force, other.Force) && Equals(this.Decline, other.Decline);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            var other = obj as DependencyConflictResolution;
            return other != null && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                return ((this.Force != null ? this.Force.GetHashCode() : 0) * 397) ^
                    (this.Decline != null ? this.Decline.GetHashCode() : 0);
            }
        }

        public static bool operator ==(DependencyConflictResolution left, DependencyConflictResolution right) {
            return Equals(left, right);
        }

        public static bool operator !=(DependencyConflictResolution left, DependencyConflictResolution right) {
            return !Equals(left, right);
        }

        #endregion
    }
}
