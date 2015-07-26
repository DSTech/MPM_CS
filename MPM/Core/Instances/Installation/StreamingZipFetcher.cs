using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;

namespace MPM.Core.Instances.Installation {
	public sealed class StreamingZipFetcher : IDisposable {
		private readonly ZipInputStream zipInputStream;

		public StreamingZipFetcher(Stream unseekableStream) {
			this.zipInputStream = new ZipInputStream(unseekableStream) { IsStreamOwner = true };
		}
		public byte[] FetchFile(String path) {
			ZipEntry entry;
			while ((entry = zipInputStream.GetNextEntry()) != null) {
				if (entry.Name != path) {
					continue;
				}
				return zipInputStream.ReadToEnd();
			}
			return null;
		}
		public void Dispose() {
			using (zipInputStream) { }
		}
	}
}
