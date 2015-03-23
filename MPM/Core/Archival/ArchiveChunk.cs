using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace MPM.Core.Archival {
	public class RawChunk : IEnumerable<byte> {
		public RawChunk(IEnumerable<byte> contents) {
			this.contents = contents.ToArray();
		}
		private byte[] contents;

		public EncryptedChunk Encrypt(byte[] encryptionKey) {
			return new EncryptedChunk(EncryptionGenerator(encryptionKey));
		}
		private IEnumerable<byte> EncryptionGenerator(byte[] encryptionKey) {
			return EnumerableEx.Concat<byte>(EncryptionGeneratorInternal(encryptionKey));
		}
		private IEnumerable<byte[]> EncryptionGeneratorInternal(byte[] encryptionKey) {
			byte[] result;
			using (var rijndael = new RijndaelManaged()) {
				rijndael.GenerateIV();
				yield return BitConverter.GetBytes(Convert.ToInt16(rijndael.IV.Length));
				yield return rijndael.IV;
				using (var deriver = new Rfc2898DeriveBytes(encryptionKey, rijndael.IV, 1000)) {
					rijndael.Key = deriver.GetBytes(32);
				}
				using (var encryptor = rijndael.CreateEncryptor()) {
					result = encryptor.TransformFinalBlock(contents, 0, contents.Length);
				}
			}
			yield return result;
		}

		public byte[] Hash() {
			using (var sha256 = new SHA256Managed()) {
				return sha256.ComputeHash(contents);
			}
		}

		public IEnumerator<byte> GetEnumerator() {
			return ((IEnumerable<byte>)contents).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<byte>)contents).GetEnumerator();
		}
	}
	public class EncryptedChunk : IEnumerable<byte> {
		public EncryptedChunk(IEnumerable<byte> contents) {
			var contentsEnumr = contents.GetEnumerator();
			var IVlength = BitConverter.ToInt16(contentsEnumr.Take(2).ToArray(), 0);
            IV = contentsEnumr.Take(IVlength).ToArray();
			this.contents = contentsEnumr.AsEnumerable().ToArray();
		}
		private byte[] IV;
		private byte[] contents;

		public RawChunk Decrypt(byte[] decryptionKey) {
			return new RawChunk(DecryptionGenerator(decryptionKey));
		}

		private IEnumerable<byte> DecryptionGenerator(byte[] decryptionKey) {
			byte[] derivedKey;
			using (var deriver = new Rfc2898DeriveBytes(decryptionKey, IV, 1000)) {
				derivedKey = deriver.GetBytes(32);
			}
			using (var rijndael = new RijndaelManaged() {
				IV = IV,
				Key = derivedKey,
			}) {
				using (var decryptor = rijndael.CreateDecryptor()) {
					return decryptor.TransformFinalBlock(contents, 0, contents.Length);
				}
			}
		}

		public byte[] Hash() {
			using (var sha256 = new SHA256Managed()) {
				return sha256.ComputeHash(contents);
			}
		}

		public IEnumerator<byte> GetEnumerator() {
			return ((IEnumerable<byte>)contents).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<byte>)contents).GetEnumerator();
		}
	}
}
