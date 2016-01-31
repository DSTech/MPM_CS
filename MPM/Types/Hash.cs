using System;
using System.Linq;
using Microsoft.Scripting.Utils;
using MPM.Util;

namespace MPM.Types {
    public class Hash {
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
            var lowerHashString = hashString.ToLower().Trim();
            var algChecksumPair = lowerHashString.Split(new[] { ':' }, 2);//Destructuring for C# 7 please?
            if (algChecksumPair.Length == 2) {
                var algorithm = algChecksumPair[0];
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
    }
}
