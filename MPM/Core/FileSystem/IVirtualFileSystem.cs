using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.FileSystem {
	/// <summary>
	/// Acts as a read-only filesystem for accessing compressed archives as directory trees.
	/// </summary>
	public interface IVirtualFileSystem {
		/// <summary>
		/// Reads a file by path from the VFS.
		/// </summary>
		/// <param name="filePath">Path to the file which shall be read</param>
		/// <returns>Stream which must be disposed after usage.</returns>
		Stream Read(String filePath);
		IEnumerable<String> FilePaths { get; }
	}
}
