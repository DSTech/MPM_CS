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
using Newtonsoft.Json.Serialization;

namespace MPM.Archival {
    public class Archive {
        public static void Create(Stream input, string packageName, Stream output, bool leaveSourceOpen = false) {
            var key = Encoding.UTF8.GetBytes(packageName);
            EncryptStreamToStream(key, input, output);
        }

        public static void Unpack(Stream input, string packageName, Stream output, bool leaveSourceOpen = false) {
            var key = Encoding.UTF8.GetBytes(packageName);
            DecryptStreamToStream(key, input, output);
        }

        private static void EncryptStreamToStream(byte[] key, Stream input, Stream outputStream) {
            using (var rijndael = new RijndaelManaged()) {
                rijndael.GenerateIV();
                using (var bsonWriter = new BsonWriter(input) { CloseOutput = false }) {
                    bsonWriter.WriteStartArray();
                    bsonWriter.WriteValue(rijndael.IV);
                    bsonWriter.WriteEnd();
                }
                using (var deriver = new Rfc2898DeriveBytes(key, rijndael.IV, 1000)) {
                    rijndael.Key = deriver.GetBytes(32);
                }
                using (var encryptor = rijndael.CreateEncryptor()) {
                    using (var crypto = new CryptoStream(input.ToReadOnly(leaveOpen: true), encryptor, CryptoStreamMode.Read)) {
                        crypto.CopyTo(outputStream);
                    }
                }
            }
        }

        private static void DecryptStreamToStream(byte[] key, Stream input, Stream outputStream) {
            byte[] derivedKey;
            byte[] iv;
            using (var bsonReader = new BsonReader(input, true, DateTimeKind.Utc) { CloseInput = false }) {
                iv = bsonReader.ReadAsBytes();
            }
            using (var deriver = new Rfc2898DeriveBytes(key, iv, 1000)) {
                derivedKey = deriver.GetBytes(32);
            }
            using (var rijndael = new RijndaelManaged() {
                IV = iv,
                Key = derivedKey,
            }) {
                using (var decryptor = rijndael.CreateDecryptor()) {
                    using (var crypto = new CryptoStream(input.ToReadOnly(leaveOpen: true), decryptor, CryptoStreamMode.Read)) {
                        crypto.CopyTo(outputStream);
                    }
                }
            }
        }
    }
}
