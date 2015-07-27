using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MPM.CLI;
using NServiceKit.Text;

namespace MPM {

	public static class StreamExtensions {

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
	}
}
