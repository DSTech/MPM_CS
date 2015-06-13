using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MPM.CLI;
using NServiceKit.Text;

namespace MPM {
	public static class IEnumerableExtensions {
		public static IEnumerable<T> SubEnumerable<T>(this IEnumerable<T> enumerable, int startIndex) {
			if (enumerable == null) {
				return null;
			}
			if (startIndex < 0) {
				throw new ArgumentOutOfRangeException(nameof(startIndex));
			}
			if (startIndex == 0) {
				return enumerable;
			}
			return enumerable.Skip(startIndex);
		}
		public static IEnumerable<T> SubEnumerable<T>(this IEnumerable<T> enumerable, int startIndex, int count) {
			if (enumerable == null) {
				return null;
			}
			if (startIndex < 0) {
				throw new ArgumentOutOfRangeException(nameof(startIndex));
			}
			if (count < 0) {
				throw new ArgumentOutOfRangeException(nameof(count));
			}
			if (count == 0) {
				return Enumerable.Empty<T>();
			}
			if (startIndex == 0) {
				return enumerable.Take(count);
			}
			return enumerable.Skip(startIndex).Take(count);
		}
	}
	public static class ActionProviderArgExtensions {
		public static MinecraftLauncher ToConfiguredLauncher(this LaunchMinecraftArgs self) {
			return new MinecraftLauncher {
				UserName = self.UserName,
			};
		}
	}
	public static class StackExtensions {
		public static bool RemoveFirst<T>(this Stack<T> stack, Predicate<T> qualifier) {
			if (stack == null) {
				throw new ArgumentNullException(nameof(stack));
			}
			if (qualifier == null) {
				throw new ArgumentNullException(nameof(qualifier));
			}
			var tempStorage = new Stack<T>(stack.Count);
			var found = false;
			while (stack.Count > 0) {
				var item = stack.Pop();
				if (qualifier.Invoke(item)) {
					found = true;
					break;
				}
				tempStorage.Push(item);
			}
			while (tempStorage.Count > 0) {
				stack.Push(tempStorage.Pop());
			}
			return found;
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
