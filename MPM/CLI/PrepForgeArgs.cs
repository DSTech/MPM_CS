using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using MPM.Types;
using PowerArgs;

namespace MPM.CLI {
    public class PrepForgeArgs : ICommandLineArgs {

        [ArgDescription("The location in which to build Forge.")]
        [ArgPosition(1)]
        [ArgDefaultValue("./forge")]
        public DirectoryInfo PackageDirectory { get; set; }

        [ArgDescription("Version of Minecraft to use.")]
        [ArgDefaultValue("latest")]
        [ArgPosition(2), ArgShortcut("-m"), ArgShortcut("--mc"), ArgShortcut("--minecraft"), ArgShortcut("--minecraftversion")]
        public SemVersion MinecraftVersion { get; set; }

        [ArgDescription("Version of Forge to use.")]
        [ArgDefaultValue("latest")]
        [ArgPosition(3), ArgShortcut("-f"), ArgShortcut("--forge"), ArgShortcut("--forgeversion")]
        public string ForgeVersion { get; set; }

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
