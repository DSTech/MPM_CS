using System;

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
				var checksum = Convert.FromBase64String(algChecksumPair[1]);

				return new Hash(algorithm, checksum);
			} else {
				var checksum = Convert.FromBase64String(algChecksumPair[0]);

				return new Hash(checksum);
			}
		}

		public override string ToString() {
			return $"{Algorithm}:{Convert.ToBase64String(Checksum)}";
		}
	}
}
