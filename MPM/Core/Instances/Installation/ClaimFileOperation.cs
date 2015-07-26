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
	public class ClaimFileOperation : IFileOperation {
		public bool UsesPreviousContents => true;

		public string PackageName { get; set; }

		public SemanticVersion PackageVersion { get; set; }

		public ClaimFileOperation() {
		}

		public ClaimFileOperation(string packageName, SemanticVersion packageVersion) {
			this.PackageName = packageName;
			this.PackageVersion = packageVersion;
		}
		
		public void Perform(IFileSystem fileSystem, String path, ICacheReader cache) {
			//A no-op
		}
	}
}
