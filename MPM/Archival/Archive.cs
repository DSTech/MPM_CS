using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MPM.Core;
using MPM.Extensions;

namespace MPM.Archival {
    public class Archive : IList<EncryptedChunk> {
        private IList<EncryptedChunk> chunks;

        public Archive(IEnumerable<EncryptedChunk> chunks) {
            this.chunks = new List<EncryptedChunk>(chunks);
        }

        public static byte[] ApplyLeadingHash(byte[] contents) {
            byte[] leadingHash;
            using (var sha256 = new SHA256Managed()) {
                leadingHash = sha256.ComputeHash(contents);
            }
            var header = BitConverter.GetBytes(Convert.ToInt16(leadingHash.Length)).Concat(leadingHash);
            return header.Concat(contents).ToArray();
        }

        public static Archive CreateArchive(string packageName, byte[] contents, uint? maxChunkSize = null) {
            contents = ApplyLeadingHash(contents);
            RawChunk[] chunks;
            if (maxChunkSize.HasValue) {
                chunks = contents
                    .Buffer(Convert.ToInt32(maxChunkSize.Value))
                    .Select(buffer => new RawChunk(buffer))
                    .ToArray();
            } else {
                chunks = new[] { new RawChunk(contents) };
            }
            var chunkKeys = new[] { Encoding.UTF8.GetBytes(packageName) }.Concat(chunks.SkipLast(1).Select(chunk => chunk.Hash()));
            var chunkKeyPairs = chunks.Zip(chunkKeys, (chunk, key) => new { chunk, key });

            var encryptedChunks = chunkKeyPairs.Select(pair => pair.chunk.Encrypt(pair.key));

            return new Archive(encryptedChunks);
        }

        //Returns null if leading-hash doesn't match the body
        public static byte[] VerifyLeadingHash(byte[] contents) {
            if (contents.Length < sizeof(Int16)) {
                throw new ArgumentOutOfRangeException(nameof(contents), "provided contents were too short");
            }
            var contentsEnumr = contents.AsEnumerable().GetEnumerator();
            var leadingHashLength = BitConverter.ToInt16(contentsEnumr.Take(2).ToArray(), 0);
            var leadingHash = contentsEnumr.Take(leadingHashLength).ToArray();
            var body = contentsEnumr.AsEnumerable().ToArray();
            using (var sha256 = new SHA256Managed()) {
                if (!sha256.ComputeHash(body).SequenceEqual(leadingHash)) {
                    return null;
                }
            }
            return body;
        }

        public byte[] Unpack(string packageName) {
            if (chunks.Count == 0) {
                throw new InvalidOperationException("No chunks to decrypt in archive");
            }
            var unpacked = UnpackInternal(packageName).Concat().ToArray();
            var verifiedBody = VerifyLeadingHash(unpacked);
            return verifiedBody;
        }

        private IEnumerable<IEnumerable<byte>> UnpackInternal(string packageName) {
            var key = Encoding.UTF8.GetBytes(packageName);
            foreach (var chunk in chunks) {
                var rawChunk = chunk.Decrypt(key);
                yield return rawChunk;
                key = rawChunk.Hash();
            }
        }

        #region IList

        public int Count => chunks.Count;

        public bool IsReadOnly => chunks.IsReadOnly;

        public EncryptedChunk this[int index] {
            get { return chunks[index]; }
            set { chunks[index] = value; }
        }

        public void Add(EncryptedChunk item) => chunks.Add(item);

        public void Clear() => chunks.Clear();

        public bool Contains(EncryptedChunk item) => chunks.Contains(item);

        public void CopyTo(EncryptedChunk[] array, int arrayIndex) => chunks.CopyTo(array, arrayIndex);

        public IEnumerator<EncryptedChunk> GetEnumerator() => chunks.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => chunks.GetEnumerator();

        public int IndexOf(EncryptedChunk item) => chunks.IndexOf(item);

        public void Insert(int index, EncryptedChunk item) => chunks.Insert(index, item);

        public bool Remove(EncryptedChunk item) => chunks.Remove(item);

        public void RemoveAt(int index) => chunks.RemoveAt(index);

        #endregion
    }
}
