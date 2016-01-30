using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MPM.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace MPM.Archival {
    public struct RawChunkContent {
        [JsonRequired, JsonProperty("body", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public byte[] Body { get; set; }
    }
    public class RawChunk {
        public readonly RawChunkContent Contents;

        public RawChunk(byte[] contents)
            : this(new System.IO.MemoryStream(contents, false)) { 
        }

        public RawChunk(Stream contents, bool leaveOpen = false) {
            using (var reader = new BsonReader(new BinaryReader(contents, Encoding.UTF8, leaveOpen), true, DateTimeKind.Utc)) {
                var serializer = new JsonSerializer();
                Contents = serializer.Deserialize<RawChunkContent>(reader);
            }
        }


        public EncryptedChunk Encrypt(byte[] encryptionKey) {
            byte[] encryptedChunk;
            using (var memStr = new MemoryStream()) {
                this.EncryptToStream(encryptionKey, memStr);
                memStr.Seek(0, SeekOrigin.Begin);
                encryptedChunk = memStr.ToArray();
            }
            return new EncryptedChunk(new MemoryStream(encryptedChunk));
        }

        public void EncryptToStream(byte[] encryptionKey, Stream outputStream) {
            using (var rijndael = new RijndaelManaged()) {
                rijndael.GenerateIV();
                outputStream.Write(BitConverter.GetBytes(Convert.ToInt16(rijndael.IV.Length)));
                outputStream.Write(rijndael.IV);
                using (var deriver = new Rfc2898DeriveBytes(encryptionKey, rijndael.IV, 1000)) {
                    rijndael.Key = deriver.GetBytes(32);
                }
                using (var encryptor = rijndael.CreateEncryptor()) {
                    using (var crypto = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write)) {
                        crypto.Write(this.Contents.Body);
                    }
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