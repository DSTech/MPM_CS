using System;

namespace MPM.Core.FileSystem {
	public interface IFileSystem {
		IFileNode Resolve(System.Uri location);
	}
}
