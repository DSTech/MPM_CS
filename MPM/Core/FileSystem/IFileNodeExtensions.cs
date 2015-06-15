using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.FileSystem {
	public static class IFileNodeExtensions {
		public static void Append(this IFileNode node, byte[] data) {
			node.Append(new ArraySegment<byte>(data));
		}
        public static void Append(this IFileNode node, ArraySegment<byte> data) {
			using (var file = node.OpenEdit()) {
				file.Seek(0, System.IO.SeekOrigin.End);
				file.Write(data.Array, data.Offset, data.Count);
			}
		}
	}
}
