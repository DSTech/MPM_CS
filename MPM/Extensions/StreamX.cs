using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MPM.Util;

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

        public static void Write(this Stream stream, byte[] byteArray) => stream.Write(byteArray, 0, byteArray.Length);

        public static ReadOnlyStream ToReadOnly(this Stream stream) => new ReadOnlyStream(stream);

        public static ReadOnlyStream ToReadOnly(this Stream stream, bool leaveOpen) => new ReadOnlyStream(stream, leaveOpen);

        public static void SeekToStart(this Stream stream) => stream.Seek(0, SeekOrigin.Begin);

        public static byte[] ToArrayFromStart(this MemoryStream memoryStream) {
            memoryStream.SeekToStart();
            return memoryStream.ToArray();
        }

        public static byte[] ToArray(this Stream stream) {
            var asMemStr = stream as MemoryStream;
            if (asMemStr != null) {
                return asMemStr.ToArray();
            }
            using (var memStr = new MemoryStream()) {
                stream.CopyTo(memStr);
                return memStr.ToArrayFromStart();
            }
        }

        public static byte[] ToArrayFromStart(this Stream stream) {
            if (!stream.CanSeek) {
                throw new ArgumentException("Does not support seeking", nameof(stream));
            }
            stream.SeekToStart();
            return stream.ToArray();
        }

    }
}
