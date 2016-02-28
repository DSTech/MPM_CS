using System.IO;
using PowerArgs;

namespace MPM.CLI {
    public class LaunchMinecraftArgs {
        [ArgRequired(PromptIfMissing = true)]
        [ArgDescription("The directory within which to initialize an instance")]
        [ArgPosition(1)]
        [ArgDefaultValue(".")]
        public DirectoryInfo InstanceDirectory { get; set; }

        [ArgDescription("The username of the profile to use")]
        [ArgShortcut("-u"), ArgShortcut("--user"), ArgShortcut("--username")]
        [ArgEnforceCase]
        public string UserName { get; set; }
    }
}
