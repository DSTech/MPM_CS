using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.Archival {
	public class Archive : IList<EncryptedChunk> {
		public Archive(IEnumerable<EncryptedChunk> chunks) {
			this.chunks = new List<EncryptedChunk>(chunks);
		}
		private IList<EncryptedChunk> chunks;

		public static byte[] ApplyLeadingHash(byte[] contents) {
			byte[] leadingHash;
			using (var sha256 = new SHA256Managed()) {
				leadingHash = sha256.ComputeHash(contents);
			}
			var header = Enumerable.Concat(
				BitConverter.GetBytes(Convert.ToInt16(leadingHash.Length)),
				leadingHash);
			return Enumerable.Concat(header, contents).ToArray();
		}
		public static byte[] VerifyLeadingHash(byte[] contents) {
			var leadingHashLength = BitConverter.ToInt16(contents.Take(2).ToArray(), 0);
			var leadingHash = contents.Skip(2).Take(leadingHashLength).ToArray();
			var body = contents.Skip(2 + leadingHashLength).ToArray();
			using (var sha256 = new SHA256Managed()) {
				if (!sha256.ComputeHash(body).SequenceEqual(leadingHash)) {
					return null;
				}
			}
			return body;
		}
		public static async Task<Archive> CreateArchive(string packageName, byte[] contents, uint? maxChunkSize = null) {
			contents = await Task.Run(() => ApplyLeadingHash(contents));
			RawChunk[] chunks;
			if (maxChunkSize.HasValue) {
				chunks = contents
					.Buffer(Convert.ToInt32(maxChunkSize.Value))
					.Select(buffer => new RawChunk(buffer))
					.ToArray();
			} else {
				chunks = new[] { new RawChunk(contents) };
			}
			var chunkKeys = Enumerable.Concat(new[] { System.Text.Encoding.UTF8.GetBytes(packageName) }, chunks.Select(chunk => chunk.Hash()).SkipLast(1));
			var chunkKeyPairs = chunks.Zip(chunkKeys, (chunk, key) => new { chunk, key });

			var encryptedChunks = chunkKeyPairs.Select((pair) => pair.chunk.Encrypt(pair.key));

			return await Task.Run(() => new Archive(encryptedChunks)).ConfigureAwait(false);
		}

		//Returns null if leading-hash verification fails
		public async Task<byte[]> Unpack(string packageName) {
			return await Task.Run(() => {
				var unpacked = EnumerableEx.Concat(UnpackInternal(packageName)).ToArray();
				var verifiedBody = VerifyLeadingHash(unpacked);
				if (verifiedBody == null) {
					return null;
				}
				return verifiedBody;
			});
		}
		private IEnumerable<IEnumerable<byte>> UnpackInternal(string packageName) {
			var key = System.Text.Encoding.UTF8.GetBytes(packageName);
			foreach (var chunk in chunks) {
				var rawChunk = chunk.Decrypt(key);
				yield return rawChunk;
				key = rawChunk.Hash();
			}
		}

		public EncryptedChunk this[int index] {
			get {
				return chunks[index];
			}

			set {
				chunks[index] = value;
			}
		}

		public int Count {
			get {
				return chunks.Count;
			}
		}

		public bool IsReadOnly {
			get {
				return chunks.IsReadOnly;
			}
		}

		public void Add(EncryptedChunk item) {
			chunks.Add(item);
		}

		public void Clear() {
			chunks.Clear();
		}

		public bool Contains(EncryptedChunk item) {
			return chunks.Contains(item);
		}

		public void CopyTo(EncryptedChunk[] array, int arrayIndex) {
			chunks.CopyTo(array, arrayIndex);
		}

		public IEnumerator<EncryptedChunk> GetEnumerator() {
			return chunks.GetEnumerator();
		}

		public int IndexOf(EncryptedChunk item) {
			return chunks.IndexOf(item);
		}

		public void Insert(int index, EncryptedChunk item) {
			chunks.Insert(index, item);
		}

		public bool Remove(EncryptedChunk item) {
			return chunks.Remove(item);
		}

		public void RemoveAt(int index) {
			chunks.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return chunks.GetEnumerator();
		}
	}
}
