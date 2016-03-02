using PowerArgs;

namespace MPM.CLI {
    public class LoginArgs : ICommandLineArgs {
        [ArgRequired(PromptIfMissing = true)]
        [ArgDescription("Username to use during login")]
        [ArgPosition(1)]
        [ArgShortcut("-u"), ArgShortcut("--user"), ArgShortcut("--username"), ArgShortcut("--name")]
        public string UserName { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        [ArgDescription("Password to use during login")]
        [ArgPosition(2)]
        [ArgShortcut("-p"), ArgShortcut("--pass"), ArgShortcut("--password")]
        public string Password { get; set; }
    }
}
