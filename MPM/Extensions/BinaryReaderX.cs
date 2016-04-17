using System.Threading.Tasks;

namespace System.IO {
    public static class BinaryReaderX {
        public static void CopyTo(this BinaryReader reader, Stream destination) {
            reader.BaseStream.CopyTo(destination);
        }
        public static void CopyTo(this BinaryReader reader, Stream destination, int bufferSize) {
            reader.BaseStream.CopyTo(destination, bufferSize);
        }
        public static Task CopyToAsync(this BinaryReader reader, Stream destination) {
            return reader.BaseStream.CopyToAsync(destination);
        }
        public static byte[] ReadToEnd(this BinaryReader reader) {
            using (var ms = new MemoryStream()) {
                reader.CopyTo(ms);
                ms.Position = 0;
                return ms.ToArray();
            }
        }

        public static async Task<byte[]> ReadToEndAsync(this BinaryReader reader) {
            using (var ms = new MemoryStream()) {
                await reader.CopyToAsync(ms);
                ms.Position = 0;
                return ms.ToArray();
            }
        }

        public static byte[] ReadToEndAndClose(this BinaryReader reader) {
            using (reader) {
                return reader.ReadToEnd();
            }
        }

        public static async Task<byte[]> ReadToEndAndCloseAsync(this BinaryReader reader) {
            using (reader) {
                return await reader.ReadToEndAsync();
            }
        }

        public static void CopyToAndClose(this BinaryReader reader, Stream destination) {
            using (reader) {
                reader.CopyTo(destination: destination);
            }
        }

        public static void CopyToAndClose(this BinaryReader reader, Stream destination, int bufferSize) {
            using (reader) {
                reader.CopyTo(destination: destination, bufferSize: bufferSize);
            }
        }

        public static async Task CopyToAndCloseAsync(this BinaryReader reader, Stream destination) {
            using (reader) {
                await reader.CopyToAsync(destination: destination);
            }
        }
    }
}
