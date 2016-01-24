using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using PowerArgs;

namespace MPM.CLI {
    public class CreatePackageArgs {
        [ArgIgnore]
        public DirectoryInfo PackageDirectory => PackageSpecFile.Directory;

        [ArgDescription("The package spec to build, in the directory containing the target contents.")]
        [ArgPosition(1)]
        [ArgDefaultValue("./package.json")]
        [ArgExistingFile]
        [ArgRegex(@"^.*[\\/]package.json$")]
        public FileInfo PackageSpecFile { get; set; }

        [ArgReviver]
        public static FileInfo ToFileInfo(string keyName, string filePath) {
            return new FileInfo(filePath, PathFormat.RelativePath);
        }
    }
}
