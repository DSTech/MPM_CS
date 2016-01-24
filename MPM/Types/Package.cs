using System;
using System.Collections.Generic;
using System.Linq;

namespace MPM.Types {
    public class Package : IEquatable<Package> {
        public Package() {
        }

        public Package(string name, IEnumerable<Author> authors, IEnumerable<Build> builds) {
            this.Name = name;
            this.Authors = authors.ToList();
            this.Builds = builds.ToList();
        }

        public String Name { get; }
        public List<Author> Authors { get; set; }
        public List<Build> Builds { get; set; }

        #region Equality members

        public bool Equals(Package other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return string.Equals(this.Name, other.Name) && Equals(this.Authors, other.Authors) &&
                Equals(this.Builds, other.Builds);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            var other = obj as Package;
            return other != null && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = this.Name != null ? this.Name.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (this.Authors != null ? this.Authors.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Builds != null ? this.Builds.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Package left, Package right) {
            return Equals(left, right);
        }

        public static bool operator !=(Package left, Package right) {
            return !Equals(left, right);
        }

        #endregion
    }
}
