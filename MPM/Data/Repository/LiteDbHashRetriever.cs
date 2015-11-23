using System;
using System.IO;
using System.Threading.Tasks;
using LiteDB;
using MPM.Extensions;

namespace MPM.Data.Repository {
	public class LiteDbHashRetriever : IHashRetriever {
		public LiteDbHashRetriever() {
		}
		public LiteDbHashRetriever(LiteFileStorage fileStorage, byte[] hash) {
			if ((this.FileStorage = fileStorage) == null) {
				throw new ArgumentNullException(nameof(fileStorage));
			}
			if ((this.Hash = hash) == null) {
				throw new ArgumentNullException(nameof(hash));
			}
		}
		public LiteFileStorage FileStorage { get; set; }
		public byte[] Hash { get; set; }

		public Task<byte[]> Retrieve() {
			return Task.FromResult(FileStorage.FindById(LiteDbHashRepository.HashToId(Hash))?.OpenRead()?.ReadToEndAndClose());
		}

		public Task<Stream> RetrieveStream() {
			return Task.Run<Stream>(() => FileStorage.FindById(LiteDbHashRepository.HashToId(Hash))?.OpenRead());
		}
	}
}
