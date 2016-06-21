using System;
using System.Collections.Generic;
using System.IO;
using MPM.Types;
using Newtonsoft.Json;
using PowerArgs;

namespace MPM.CLI {
    public class InstallArgs : ICommandLineArgs {

        [ArgDescription("The directory of the target instance")]
        [ArgShortcut("-i"), ArgShortcut("--instance")]
        [ArgDefaultValue(".")]
        public DirectoryInfo InstanceDirectory { get; set; }

        [ArgRequired(PromptIfMissing = false)]
        [ArgDescription("The package or packages to install")]
        [ArgPosition(1)]
        public List<String> Packages { get; set; }

        #region Arg Revivers

        [ArgReviver]
        public static InstanceSide ReviveInstanceSide(string paramName, string instanceSide) {
            return JsonConvert.DeserializeObject<InstanceSide>(instanceSide);
        }

        [ArgReviver]
        public static FileInfo ReviveFileInfo(string paramName, string filePath) {
            return new FileInfo(filePath);
        }

        [ArgReviver]
        public static DirectoryInfo ReviveDirectoryInfo(string paramName, string directoryPath) {
            return new DirectoryInfo(directoryPath);
        }

        [ArgReviver]
        public static MPM.Types.SemVersion ReviveArchString(string paramName, string versionString) {
            versionString = versionString.Trim();
            if (versionString == "latest") {
                return GetLatestMinecraftArch();
            }
            return new MPM.Types.SemVersion(versionString, true);
        }

        private static MPM.Types.SemVersion GetLatestMinecraftArch() {
            //TODO: Implement
            return new Types.SemVersion(1, 8, 9);
        }

        #endregion

    }
}
