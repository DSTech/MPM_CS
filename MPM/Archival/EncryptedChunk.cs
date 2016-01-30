using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MPM.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace MPM.Archival {
    public struct EncryptedChunkContent {
        [JsonRequired, JsonProperty("iv", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public byte[] IV { get; set; }
        [JsonRequired, JsonProperty("body", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public byte[] Body { get; set; }
    }
    public class EncryptedChunk {
        public readonly EncryptedChunkContent Contents;

        public int Length => Contents.Body.Length;

        public EncryptedChunk(byte[] contents)
            : this(new System.IO.MemoryStream(contents, false)) {
        }

        public EncryptedChunk(Stream contents, bool leaveOpen = false) {
            using (var reader = new BsonReader(new BinaryReader(contents, Encoding.UTF8, leaveOpen), true, DateTimeKind.Utc)) {
                var serializer = new JsonSerializer();
                Contents = serializer.Deserialize<EncryptedChunkContent>(reader);
            }
        }

        public RawChunk Decrypt(byte[] decryptionKey) {
            byte[] decrypted;
            using (var memStr = new MemoryStream()) {
                this.DecryptToStream(decryptionKey, memStr);
                memStr.Seek(0, SeekOrigin.Begin);
                decrypted = memStr.ToArray();
            }
            return new RawChunk(new MemoryStream(decrypted));
        }

        public void DecryptToStream(byte[] decryptionKey, Stream outputStream) {
            byte[] derivedKey;
            using (var deriver = new Rfc2898DeriveBytes(decryptionKey, Contents.IV, 1000)) {
                derivedKey = deriver.GetBytes(32);
            }
            using (var rijndael = new RijndaelManaged() {
                IV = Contents.IV,
                Key = derivedKey,
            }) {
                using (var crypto = new CryptoStream(new MemoryStream(Contents.Body), rijndael.CreateDecryptor(), CryptoStreamMode.Read)) {
                    crypto.CopyTo(outputStream);
                }
            }
        }

        public byte[] Hash() {
            using (var sha256 = new SHA256Managed()) {
                return sha256.ComputeHash(this.Contents.Body);
            }
        }
    }
}
