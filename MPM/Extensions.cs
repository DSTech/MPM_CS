using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using MPM.CLI;
using NServiceKit.Text;

namespace MPM {
	public static class ActionProviderArgExtensions {
		public static MinecraftLauncher ToConfiguredLauncher(this LaunchMinecraftArgs self) {
			return new MinecraftLauncher {
				UserName = self.UserName,
			};
		}
	}
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
