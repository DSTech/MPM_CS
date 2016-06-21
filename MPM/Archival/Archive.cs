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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using MsgPack;
using MsgPack.Serialization;

namespace MPM.Archival {
    public class Archive {
        public static byte[] ArchiveHeader = Encoding.UTF8.GetBytes("MPK");

        public static void Create(Stream input, string packageName, Stream output, bool leaveSourceOpen = false) {
            var key = Encoding.UTF8.GetBytes(packageName);
            output.Write(ArchiveHeader);
            output.WriteByte(2);//Format Version
            if (leaveSourceOpen) {
                EncryptStreamToStream(key, input, output);
            } else {
                using (input) {
                    EncryptStreamToStream(key, input, output);
                }
            }
        }

        public static void Unpack(Stream input, string packageName, Stream output, bool leaveSourceOpen = false) {
            var header = new byte[ArchiveHeader.Length];
            input.Read(header, 0, ArchiveHeader.Length);
            if (!header.SequenceEqual(ArchiveHeader)) {
                throw new FormatException("Archive Header not found");
            }
            var formatVersion = input.ReadByte();
            if (formatVersion != 2) {
                throw new FormatException("Archive Format Version unsupported");
            }

            var key = Encoding.UTF8.GetBytes(packageName);
            if (leaveSourceOpen) {
                DecryptStreamToStream(key, input, output);
            } else {
                using (input) {
                    DecryptStreamToStream(key, input, output);
                }
            }
        }

        private static void EncryptStreamToStream(byte[] key, Stream input, Stream outputStream) {
            using (var rijndael = new RijndaelManaged()) {
                rijndael.GenerateIV();
                using (var bsonWriter = new BsonWriter(outputStream) { CloseOutput = false }) {
                    new JArray(rijndael.IV).WriteTo(bsonWriter);
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
            using (var memstr = new MemoryStream()) {
                input.CopyTo(memstr);
                memstr.SeekToStart();
                byte[] iv, derivedKey;
                using (var bsonReader = new BsonReader(memstr, true, DateTimeKind.Utc) { CloseInput = false }) {
                    var j = JToken.ReadFrom(bsonReader);
                    iv = j[0].ToObject<byte[]>();
                }
                using (var deriver = new Rfc2898DeriveBytes(key, iv, 1000)) {
                    derivedKey = deriver.GetBytes(32);
                }
                using (var rijndael = new RijndaelManaged() {
                    IV = iv,
                    Key = derivedKey,
                }) {
                    using (var decryptor = rijndael.CreateDecryptor()) {
                        using (var crypto = new CryptoStream(memstr.ToReadOnly(leaveOpen: true), decryptor, CryptoStreamMode.Read)) {
                            crypto.CopyTo(outputStream);
                        }
                    }
                }
            }
        }
    }
}
