using System;
using System.Collections.Generic;
using System.Linq;

namespace MPM.Types {
    public class InstallationConflictResolution : IEquatable<InstallationConflictResolution> {
        public InstallationConflictResolution() {
        }

        public InstallationConflictResolution(IEnumerable<string> packageNames) {
            this.PackageNames = packageNames.ToList();
        }

        public List<String> PackageNames { get; set; }

        #region Equality members

        public bool Equals(InstallationConflictResolution other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return Equals(this.PackageNames, other.PackageNames);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            var other = obj as InstallationConflictResolution;
            return other != null && Equals(other);
        }

        public override int GetHashCode() {
            return this.PackageNames != null ? this.PackageNames.GetHashCode() : 0;
        }

        public static bool operator ==(InstallationConflictResolution left, InstallationConflictResolution right) {
            return Equals(left, right);
        }

        public static bool operator !=(InstallationConflictResolution left, InstallationConflictResolution right) {
            return !Equals(left, right);
        }

        #endregion
    }
}
