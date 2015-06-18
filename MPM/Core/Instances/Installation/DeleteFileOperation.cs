using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform.VirtualFileSystem;

namespace MPM.Core.Instances.Installation {
	public class DeleteFileOperation : IFileOperation {
		public bool Reversible {
			get {
				return false;
			}
		}

		public bool UsesPreviousContents {
			get {
				return false;
			}
		}

		public void Perform(IFileSystem fileSystem, String path) {
			fileSystem.ResolveFile(path).Delete();
		}

		public void Reverse(IFileSystem fileSystem, String path) {
			Debug.Assert(false);
			throw new NotSupportedException();
		}
	}
}
