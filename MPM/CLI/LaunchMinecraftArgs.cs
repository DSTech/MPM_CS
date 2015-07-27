using PowerArgs;

namespace MPM.CLI {

	public class LaunchMinecraftArgs {

		[ArgDescription("The username of the profile to use")]
		[ArgShortcut("-u"), ArgShortcut("--user"), ArgShortcut("--username")]
		[ArgEnforceCase]
		public string UserName { get; set; }
	}
}
