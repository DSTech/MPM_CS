using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Types {

	public class Hash {

		public Hash(string algorithm, byte[] checksum) {
			this.Algorithm = algorithm;
			this.Checksum = checksum;
		}

		public Hash(byte[] checksum) {
			this.Algorithm = "sha256";
			this.Checksum = checksum;
		}

		public String Algorithm { get; }

		public Byte[] Checksum { get; }

		public static Hash Parse(string hashString) {
			var _hashString = hashString.ToLower().Trim();

			var algChecksumPair = _hashString.Split(new[] { ':' }, 2);//Destructuring for C# 7 please?

			if (algChecksumPair.Length == 2) {
				var algorithm = algChecksumPair[0];
				var checksum = Convert.FromBase64String(algChecksumPair[1]);

				return new Hash(algorithm: algorithm, checksum: checksum);
			} else {
				var checksum = Convert.FromBase64String(algChecksumPair[0]);

				return new Hash(checksum: checksum);
			}
		}

		public override string ToString() {
			return $"{Algorithm}:{Convert.ToBase64String(Checksum)}";
		}
	}
}
