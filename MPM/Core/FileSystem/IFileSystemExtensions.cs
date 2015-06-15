using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.FileSystem {
	public static class IFileSystemExtensions {
		public static bool Delete(this IFileSystem fileSystem, Uri location) {
			var node = fileSystem.Resolve(location);
			return node.Delete();
		}
	}
}
