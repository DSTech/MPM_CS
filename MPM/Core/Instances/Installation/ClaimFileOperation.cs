using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Cache;
using Platform.VirtualFileSystem;

namespace MPM.Core.Instances.Installation {
    public class ClaimFileOperation : IFileOperation {
        public ClaimFileOperation() {
        }

        public ClaimFileOperation(string packageName, MPM.Types.SemVersion packageVersion) {
            this.PackageName = packageName;
            this.PackageVersion = packageVersion;
        }

        public bool UsesPreviousContents => true;

        public string PackageName { get; set; }

        public MPM.Types.SemVersion PackageVersion { get; set; }

        public void Perform(IFileSystem fileSystem, String path, ICacheReader cache) {
            //A no-op
        }

        public override string ToString() => $"<Claim>";
    }
}
