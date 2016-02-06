using System.IO;
using Newtonsoft.Json;
using PowerArgs;

namespace MPM.CLI {
    public class InitArgs {
        [ArgRequired(PromptIfMissing = true)]
        [ArgDescription("The directory within which to initialize an instance")]
        [ArgPosition(1)]
        [ArgDefaultValue(".")]
        public DirectoryInfo InstanceDirectory { get; set; }

        [ArgShortcut("-f"), ArgShortcut("--force"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly)]
        [ArgDefaultValue(false)]
        public bool ForceNonEmptyInstancePath { get; set; }

        [ArgDescription("The minecraft version to use, eg 1.8")]
        [ArgDefaultValue("latest")]
        [ArgShortcut("-a"), ArgShortcut("--arch"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly)]
        public MPM.Types.SemVersion Arch { get; set; }

        [ArgDescription("The minecraft side, eg server, client")]
        [ArgDefaultValue("client")]
        [ArgShortcut("-s"), ArgShortcut("--side"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly)]
        public InstanceSide Side { get; set; }

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
