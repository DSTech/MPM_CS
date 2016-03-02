using System.IO;
using PowerArgs;

namespace MPM.CLI {
    public class LaunchMinecraftArgs : ICommandLineArgs {
        [ArgRequired(PromptIfMissing = true)]
        [ArgDescription("The directory of the instance to launch")]
        [ArgPosition(1)]
        [ArgDefaultValue(".")]
        public DirectoryInfo InstanceDirectory { get; set; }

        [ArgDescription("The username of the profile to use, defaulting to any existing profile")]
        [ArgShortcut("-u"), ArgShortcut("--user"), ArgShortcut("--username")]
        [ArgEnforceCase]
        public string UserName { get; set; }
    }
}
