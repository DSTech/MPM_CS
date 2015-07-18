using PowerArgs;

namespace MPM.CLI {
	public class InitArgs {
		[ArgRequired(PromptIfMissing = true)]
		[ArgDescription("The directory within which to initialize an instance")]
		[ArgPosition(1)]
		[ArgExistingDirectory]
		[ArgDefaultValue(".")]
		public string InstancePath { get; set; }

		[ArgShortcut("-f"), ArgShortcut("--force"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly)]
		public bool ForceNonEmptyInstancePath { get; set; }

		[ArgDescription("The minecraft version to use, eg 1.8")]
		[ArgDefaultValue("latest")]
		[ArgShortcut("-a"), ArgShortcut("--arch"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly)]
		public string Arch { get; set; }

		[ArgDescription("The minecraft side, eg server, client")]
		[ArgDefaultValue("client")]
		[ArgShortcut("-s"), ArgShortcut("--side"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly)]
		public string Side { get; set; }

		[ArgDescription("The platform to use, eg current, lin32, lin64, win32, win64")]
		[ArgDefaultValue("current")]
		[ArgShortcut("-p"), ArgShortcut("--platform"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly)]
		public string Platform { get; set; }
	}
}
