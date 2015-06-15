using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.FileSystem;

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

		public void Perform(IFileSystem fileSystem, Uri uri) {
			fileSystem.Delete(uri);
		}

		public void Reverse(IFileSystem fileSystem, Uri uri) {
			Debug.Assert(false);
			throw new NotSupportedException();
		}
	}
}
