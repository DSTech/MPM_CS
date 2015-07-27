using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Cache;
using Platform.VirtualFileSystem;
using semver.tools;

namespace MPM.Core.Instances.Installation {

	public class DeleteFileOperation : IFileOperation {
		public bool UsesPreviousContents => false;

		public string PackageName { get; set; }

		public SemanticVersion PackageVersion { get; set; }

		public DeleteFileOperation() {
		}

		public DeleteFileOperation(string packageName, SemanticVersion packageVersion) {
			this.PackageName = packageName;
			this.PackageVersion = packageVersion;
		}

		public void Perform(IFileSystem fileSystem, String path, ICacheReader cache) {
			fileSystem.ResolveFile(path).Delete();
		}
	}
}
