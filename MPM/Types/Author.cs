using System;
using Newtonsoft.Json;

namespace MPM.Types {
    public class Author : IEquatable<Author> {
        public Author() {
        }

        public Author(string name, string email) {
            this.Name = name;
            this.Email = email;
        }

        [JsonProperty("name")]
        public String Name { get; set; } = "";

        [JsonProperty("email")]
        public String Email { get; set; } = "";

        public override string ToString() {
            var nameExists = !String.IsNullOrWhiteSpace(Name);
            var emailExists = !String.IsNullOrWhiteSpace(Email);
            if (nameExists) {
                if (emailExists) {
                    return $"{Name} <{Email}>";
                }
                return this.Name;
            }
            if (emailExists) {
                return $"<{this.Email}>";
            }
            return "";
        }

        #region Equality members

        public bool Equals(Author other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return string.Equals(this.Name, other.Name) && string.Equals(this.Email, other.Email);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            var other = obj as Author;
            return other != null && Equals(other);
        }

        public override int GetHashCode() {
            unchecked { return ((this.Name?.GetHashCode() ?? 0) * 397) ^ (this.Email?.GetHashCode() ?? 0); }
        }

        public static bool operator ==(Author left, Author right) {
            return Equals(left, right);
        }

        public static bool operator !=(Author left, Author right) {
            return !Equals(left, right);
        }

        #endregion
    }
}
