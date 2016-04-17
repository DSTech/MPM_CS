using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace MPM.Util {
    public sealed class StreamingZipFetcher : IDisposable {
        private readonly ZipInputStream zipInputStream;

        public StreamingZipFetcher(Stream unseekableStream) {
            this.zipInputStream = new ZipInputStream(unseekableStream) { IsStreamOwner = true };
        }

        public void Dispose() {
            using (zipInputStream) { }
        }

        /// <summary>
        ///     Enumerates every file in the archive.
        /// </summary>
        /// <returns>
        ///     Sequence of streams which may be bypassed to continue to their successor or close the
        ///     <see cref="StreamingZipFetcher" />.
        ///     Read all elements from the result before disposing of the fetcher.
        /// </returns>
        public IEnumerable<Tuple<ZipEntry, Stream>> EnumerateFiles() {
            ZipEntry entry;
            while ((entry = zipInputStream.GetNextEntry()) != null) {
                yield return new Tuple<ZipEntry, Stream>(entry, zipInputStream);
            }
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
    }
}
