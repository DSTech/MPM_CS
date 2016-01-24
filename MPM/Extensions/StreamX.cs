using System;
using System.IO;
using System.Threading.Tasks;

namespace MPM.Extensions {
    public static class StreamX {
        public static byte[] ReadToEnd(this Stream stream) {
            using (var ms = new MemoryStream()) {
                stream.CopyTo(ms);
                ms.Position = 0;
                return ms.ToArray();
            }
        }

        public static async Task<byte[]> ReadToEndAsync(this Stream stream) {
            using (var ms = new MemoryStream()) {
                await stream.CopyToAsync(ms);
                ms.Position = 0;
                return ms.ToArray();
            }
        }

        public static DisposerStreamWrapper AndDispose(this Stream stream, params IDisposable[] additionalDisposables) {
            return new DisposerStreamWrapper(stream, additionalDisposables);
        }

        public static DisposerStreamWrapper AndDispose(this DisposerStreamWrapper stream, params IDisposable[] additionalDisposables) {
            stream.AddDisposables(additionalDisposables);
            return stream;
        }

        public static byte[] ReadToEndAndClose(this Stream stream) {
            using (stream) {
                return stream.ReadToEnd();
            }
        }

        public static async Task<byte[]> ReadToEndAndCloseAsync(this Stream stream) {
            using (stream) {
                return await stream.ReadToEndAsync();
            }
        }

        public static void CopyToAndClose(this Stream stream, Stream destination) {
            using (stream) {
                stream.CopyTo(destination: destination);
            }
        }

        public static void CopyToAndClose(this Stream stream, Stream destination, int bufferSize) {
            using (stream) {
                stream.CopyTo(destination: destination, bufferSize: bufferSize);
            }
        }

        public static async Task CopyToAndCloseAsync(this Stream stream, Stream destination) {
            using (stream) {
                await stream.CopyToAsync(destination: destination);
            }
        }
    }
}
