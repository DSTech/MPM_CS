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
        [ArgDefaultValue("1.9.4")]
        [ArgPosition(2), ArgShortcut("-m"), ArgShortcut("--mc"), ArgShortcut("--minecraft"), ArgShortcut("--minecraftversion")]
        public string MinecraftVersion { get; set; }

        [ArgDescription("Version of Forge to use.")]
        [ArgDefaultValue("latest")]
        [ArgPosition(3), ArgShortcut("-f"), ArgShortcut("--forge"), ArgShortcut("--forgeversion")]
        public string ForgeVersion { get; set; }

        [ArgDescription("Side for compatibility (eg client/server/universal)")]
        [ArgDefaultValue("universal")]
        [ArgPosition(4), ArgShortcut("-s"), ArgShortcut("--side")]
        public CompatibilitySide Side { get; set; }

        [ArgReviver]
        public static FileInfo ReviveFileInfo(string keyName, string filePath) {
            return new FileInfo(filePath, PathFormat.RelativePath);
        }

        [ArgReviver]
        public static DirectoryInfo ReviveDirectoryInfo(string keyName, string filePath) {
            return new DirectoryInfo(filePath, PathFormat.RelativePath);
        }

        [ArgReviver]
        public static CompatibilitySide ReviveSide(string keyName, string sideString) {
            switch (sideString.ToLowerInvariant()) {
                case "c":
                case "client":
                    return CompatibilitySide.Client;
                case "s":
                case "server":
                    return CompatibilitySide.Server;
                case "u":
                case "universal":
                    return CompatibilitySide.Universal;
                default:
                    throw new KeyNotFoundException("Side specifier not found");
            }
        }
    }
}
