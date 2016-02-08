using System;
using Newtonsoft.Json;

namespace MPM.Types {
    [JsonConverter(typeof(MPM.Util.Json.ArchConverter))]
    public class Arch : IEquatable<Arch>, IComparable<Arch> {
        public Arch() {
        }

        public Arch(string archId) {
            this.Id = new SemVersion(archId, true);
        }

        public Arch(SemVersion archId) {
            this.Id = archId;
        }

        public SemVersion Id { get; set; }

        public override string ToString() {
            return Id.ToString();
        }

        public static implicit operator SemVersion(Arch arch) {
            return arch.Id;
        }

        public static implicit operator Arch(SemVersion version) {
            return new Arch(version);
        }

        #region Equality members

        public bool Equals(Arch other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return this.Id == other.Id;
        }

        public int CompareTo(Arch other) {
            return this.Id.CompareTo(other.Id);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            var other = obj as Arch;
            return other != null && Equals(other);
        }

        public override int GetHashCode() {
            return this.Id?.GetHashCode() ?? 0;
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
