using PowerArgs;

namespace MPM.CLI {
    public class UpdateCauldronArgs {
        [ArgRequired(PromptIfMissing = true)]
        [ArgDescription("The Cauldron installer file to update")]
        [ArgPosition(1)]
        [ArgExistingFile]
        public string CauldronFile { get; set; }
    }
}
