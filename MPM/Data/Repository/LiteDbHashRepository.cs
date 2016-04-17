using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LiteDB;

namespace MPM.Data.Repository {
    static class HexStrings {
        public static string[] HexTbl = Enumerable.Range(0, 256).Select(v => v.ToString("X2")).ToArray();

        public static string ToHex(this IEnumerable<byte> array) {
            StringBuilder s = new StringBuilder();
            foreach (var v in array) {
                s.Append(HexTbl[v]);
            }
            return s.ToString();
        }

        public static string ToHex(this byte[] array) {
            StringBuilder s = new StringBuilder(array.Length * 2);
            foreach (var v in array) {
                s.Append(HexTbl[v]);
            }
            return s.ToString();
        }
    }

    public class LiteDbHashRepository : IHashRepository {
        public const string ID_PREFIX = "/hashes/";
        private readonly LiteFileStorage HashFileStorage;

        public LiteDbHashRepository(LiteFileStorage hashFileStorage) {
            if ((this.HashFileStorage = hashFileStorage) == null) {
                throw new ArgumentNullException(nameof(hashFileStorage));
            }
        }

        public IEnumerable<IHashRetriever> Resolve(IEnumerable<byte[]> hashes) {
            return CreateRetrievers(hashes);
        }

        public static string HashToId(byte[] hash) {
            return hash.ToHex();//$"{ID_PREFIX}{hash.ToHex()}";
        }

        private IHashRetriever CreateRetriever(byte[] hash) {
            return new LiteDbHashRetriever(HashFileStorage, hash);
        }

        private IEnumerable<IHashRetriever> CreateRetrievers(IEnumerable<byte[]> hashes) {
            return hashes.Select(CreateRetriever).ToArray();
        }

        public byte[] Register(byte[] hash, Stream content) {
            var id = HashToId(hash);
            using (content) {
                HashFileStorage.Upload(HashToId(hash), content);
            }
            return hash;
        }
    }
}
