using System;
using System.Linq;
using Microsoft.Scripting.Utils;
using MPM.Util;
using Newtonsoft.Json;

namespace MPM.Types {
    [JsonConverter(typeof(Util.Json.HashConverter))]
    public class Hash : IEquatable<Hash> {
        public Hash() {
        }

        public Hash(string algorithm, byte[] checksum) {
            this.Algorithm = algorithm;
            this.Checksum = checksum;
        }

        public Hash(byte[] checksum) {
            this.Algorithm = "sha256";
            this.Checksum = checksum;
        }

        public String Algorithm { get; set; }

        public Byte[] Checksum { get; set; }

        public static Hash Parse(string hashString) {
            var trimmed = hashString.Trim();
            var algChecksumPair = trimmed.Split(new[] { ':' }, 2);//Destructuring for C# 7 please?
            if (algChecksumPair.Length == 2) {
                var algorithm = algChecksumPair[0].ToLowerInvariant();
                var checksum = Base64.GetBytesUnknown(algChecksumPair[1]);
                return new Hash(algorithm, checksum);
            } else {
                var checksum = Base64.GetBytesUnknown(algChecksumPair[0]);
                return new Hash(checksum);
            }
        }

        public override string ToString() {
            return $"{Algorithm}:{Base64.GetSafeString(Checksum)}";
        }

        #region Equality members

        public bool Equals(Hash other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return string.Equals(this.Algorithm, other.Algorithm, StringComparison.InvariantCulture) && this.Checksum.SequenceEqual(other.Checksum);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            var other = obj as Hash;
            return other != null && Equals(other);
        }

        public override int GetHashCode() {
            unchecked { return ((this.Algorithm != null ? StringComparer.InvariantCulture.GetHashCode(this.Algorithm) : 0) * 397) ^ (this.Checksum != null ? this.Checksum.GetHashCode() : 0); }
        }

        public static bool operator ==(Hash left, Hash right) {
            return Equals(left, right);
        }

        public static bool operator !=(Hash left, Hash right) {
            return !Equals(left, right);
        }

        #endregion
    }
}
