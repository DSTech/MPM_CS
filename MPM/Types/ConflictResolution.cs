using System;
using Newtonsoft.Json;

namespace MPM.Types {
    public class ConflictResolution : IEquatable<ConflictResolution> {
        public ConflictResolution() {
        }

        public ConflictResolution(DependencyConflictResolution dependency, InstallationConflictResolution installation) {
            this.Dependency = dependency;
            this.Installation = installation;
        }

        public DependencyConflictResolution Dependency { get; set; }

        public InstallationConflictResolution Installation { get; set; }

        public bool ShouldSerializeDependency() => Dependency?.ShouldSerialize() == true;

        public bool ShouldSerializeInstallation() => Installation?.ShouldSerialize() == true;

        public bool ShouldSerialize() => ShouldSerializeDependency() || ShouldSerializeInstallation();

        #region Equality members

        public bool Equals(ConflictResolution other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return Equals(this.Dependency, other.Dependency) && Equals(this.Installation, other.Installation);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            var other = obj as ConflictResolution;
            return other != null && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                return ((this.Dependency != null ? this.Dependency.GetHashCode() : 0) * 397) ^
                    (this.Installation != null ? this.Installation.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ConflictResolution left, ConflictResolution right) {
            return Equals(left, right);
        }

        public static bool operator !=(ConflictResolution left, ConflictResolution right) {
            return !Equals(left, right);
        }

        #endregion
    }
}
