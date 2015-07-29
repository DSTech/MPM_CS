using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using MPM.Extensions;

namespace MPM.Archival {

	public sealed class SeekingZipFetcher : IDisposable {
		private readonly ZipFile zipFile;

		public SeekingZipFetcher(Stream seekableStream) {
			if (!seekableStream.CanSeek) {
				throw new ArgumentException("is not seekable", nameof(seekableStream));
			}
			this.zipFile = new ZipFile(seekableStream) { IsStreamOwner = true };
		}

		public void Dispose() {
			using (zipFile) { }
		}

		public byte[] FetchFile(String path) {
			var entry = zipFile.GetEntry(path);
			if (entry == null) {
				return null;
			}
			using (var zipStream = zipFile.GetInputStream(entry)) {
				return zipStream.ReadToEnd();
			}
		}

		public Stream FetchFileStream(String path) {
			var entry = zipFile.GetEntry(path);
			if (entry == null) {
				return null;
			}
			return zipFile.GetInputStream(entry);
		}
	}
}
