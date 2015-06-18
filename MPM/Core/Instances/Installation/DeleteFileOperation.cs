using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Cache;
using Platform.VirtualFileSystem;

namespace MPM.Core.Instances.Installation {
	public class DeleteFileOperation : IFileOperation {
		public bool Reversible => false;

		public bool UsesPreviousContents => false;

		public void Perform(IFileSystem fileSystem, String path, ICacheReader cache) {
			fileSystem.ResolveFile(path).Delete();
		}

		public void Reverse(IFileSystem fileSystem, String path, ICacheReader cache) {
			Debug.Assert(false);
			throw new NotSupportedException();
		}
	}
}
