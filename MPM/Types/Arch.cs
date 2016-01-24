using System;

namespace MPM.Types {
    public class Arch : IEquatable<Arch> {
        public Arch() {
        }

        public Arch(string archId) {
            this.Id = archId;
        }

        public string Id { get; set; }

        #region Equality members

        public bool Equals(Arch other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return string.Equals(this.Id, other.Id);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            var other = obj as Arch;
            return other != null && Equals(other);
        }

        public override int GetHashCode() {
            return this.Id != null ? this.Id.GetHashCode() : 0;
        }

        public static bool operator ==(Arch left, Arch right) {
            return Equals(left, right);
        }

        public static bool operator !=(Arch left, Arch right) {
            return !Equals(left, right);
        }

        #endregion
    }
}
