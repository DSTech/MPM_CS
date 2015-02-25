using PowerArgs;

namespace MPM.CLI {
	public class UpdateForgeArgs {
		[ArgRequired(PromptIfMissing = true)]
		[ArgDescription("The Forge Directory to update")]
		[ArgPosition(1)]
		[ArgExistingDirectory]
		[DefaultValue(".")]
		public string ForgeDirectory { get; set; }
	}
}
