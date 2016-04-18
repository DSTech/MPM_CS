using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using PowerArgs;

namespace MPM.CLI {
    public class CreatePackageArgs : ICommandLineArgs {
        [ArgIgnore]
        public DirectoryInfo PackageDirectory => PackageSpecFile.Directory;

        [ArgDescription("The package spec to build, in the directory containing the target contents.")]
        [ArgPosition(1)]
        [ArgDefaultValue("./package.json")]
        [ArgExistingFile]
        [ArgRegex(@"^.*[\\/]package.json$")]
        public FileInfo PackageSpecFile { get; set; }

        [ArgDescription("The maximum length of a chunk, in bytes, into which the resulting package should be split.")]
        [ArgDefaultValue(1024*1024*5)]
        [ArgShortcut("-c"), ArgShortcut("--split"), ArgShortcut("--max-chunk-length")]
        public uint MaximumChunkLength { get; set; }

        [ArgReviver]
        public static FileInfo ReviveFileInfo(string keyName, string filePath) {
            return new FileInfo(filePath, PathFormat.RelativePath);
        }

        [ArgReviver]
        public static DirectoryInfo ReviveDirectoryInfo(string keyName, string filePath) {
            return new DirectoryInfo(filePath, PathFormat.RelativePath);
        }
    }
}
