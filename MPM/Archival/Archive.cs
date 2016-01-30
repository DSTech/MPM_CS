using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MPM.Core;
using MPM.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Serialization;

namespace MPM.Archival {
    //TODO: Make archives use Hash class, and make archive construction and unpacking use streams instead of iterables
    //TODO: Split chunks by initial size, rather than resultant-size-pre-encryption
    public class Archive : IList<EncryptedChunk> {
        private IList<EncryptedChunk> chunks;
        public Archive(IEnumerable<EncryptedChunk> chunks) {
            this.chunks = new List<EncryptedChunk>(chunks);
        }

        private static byte[] ApplyLeadingHash(byte[] contents) {
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
                    .Select(buffer => new RawChunk(buffer.ToArray()))
                    .ToArray();
            } else {
                chunks = new[] { new RawChunk(contents) };
            }
            var chunkKeys = new[] { Encoding.UTF8.GetBytes(packageName) }.Concat(chunks.SkipLast(1).Select(chunk => chunk.Hash()));
            var chunkKeyPairs = chunks.Zip(chunkKeys, (chunk, key) => new { chunk, key });

            var encryptedChunks = chunkKeyPairs.Select(pair => pair.chunk.Encrypt(pair.key));

            return new Archive(encryptedChunks);
        }

        /// <summary>
        /// Verifies the leading hash of a stream.
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <returns>True if the content passed validation. When true, also seeks to the next character following the leading hash, else to the initial position.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Provided contents were too short</exception>
        private static bool VerifyLeadingHash(Stream contents) {
            var initialPosition = contents.Position;
            long endOfHash;
            using (var reader = new BinaryReader(contents, Encoding.UTF8, leaveOpen: true)) {
                var leadingHashLength = reader.ReadInt16();
                var leadingHash = reader.ReadBytes(leadingHashLength);
                endOfHash = reader.BaseStream.Position;
                var body = reader.ReadToEnd();
                using (var sha256 = new SHA256Managed()) {
                    if (!sha256.ComputeHash(body).SequenceEqual(leadingHash)) {
                        reader.BaseStream.Position = initialPosition;
                        return false;
                    }
                }
            }
            contents.Position = endOfHash;
            return true;
        }

        /// <summary>
        /// Unpacks the package to a byte array.
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns>Null if validation for the package failed, otherwise the package contents.</returns>
        public byte[] Unpack(string packageName) {
            if (this.chunks.Count == 0) {
                throw new InvalidOperationException("No chunks to decrypt in archive");
            }
            var key = Encoding.UTF8.GetBytes(packageName);
            byte[] unpacked;
            using (var memStr = new MemoryStream(this.chunks.Aggregate(0, (last, chunk) => last + chunk.Length))) {
                UnzipDecryptChunksToStream(key, memStr);
                memStr.Seek(0, SeekOrigin.Begin);
                if (VerifyLeadingHash(memStr)) {
                    return null;
                }
                unpacked = memStr.ToArray();
            }
            return unpacked;
        }

        public void UnpackChunksToStream(string packageName, Stream outputStream) {
            if (this.chunks.Count == 0) {
                throw new InvalidOperationException("No chunks to decrypt in archive");
            }
            var key = Encoding.UTF8.GetBytes(packageName);
            byte[] unpacked;
            using (var memStr = new MemoryStream(this.chunks.Aggregate(0, (last, chunk) => last + chunk.Length))) {
                UnzipDecryptChunksToStream(key, memStr);
                memStr.Seek(0, SeekOrigin.Begin);
                if (VerifyLeadingHash(memStr)) {
                    return null;
                }
                unpacked = memStr.ToArray();
            }
            return unpacked;
        }

        //Does not validate content if it passes encryption 
        protected void UnzipDecryptChunksToStream(byte[] key, Stream outputStream) {
            using (var hashingStream = new HashingForwarderStream(outputStream: outputStream, ownsStream: false)) {
                chunks.Aggregate(key, (thisKey, chunk) => {
                    chunk.DecryptToStream(thisKey, outputStream);
                    return hashingStream.HashBytes;
                });
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
