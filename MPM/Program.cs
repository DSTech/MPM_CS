using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArgs;

namespace MPM {
	[ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
	public class LaunchArgs {
		[ArgDescription("Prints all messages to stdout")]
		[ArgShortcut("--verbose"), ArgShortcut("-v")]
		[ArgEnforceCase]
		public bool Verbose { get; set; }

		[HelpHook]
		[ArgShortcut("--help"), ArgShortcut("-h")]
		[ArgEnforceCase]
		public bool Help { get; set; }

		[ArgActionMethod]
		[ArgShortcut("--uc")]
		public void UpdateCauldron(UpdateCauldronArgs args) {
			Console.WriteLine("UpdateCauldron called, file was " + args.CauldronFile);
		}
	}
	public class UpdateCauldronArgs {
		[ArgRequired(PromptIfMissing = true)]
		[ArgDescription("The Cauldron installer file to update")]
		[ArgPosition(1)]
		[ArgExistingFile]
		public string CauldronFile { get; set; }
	}
	public class Program {
		public static void Main(string[] args) {
			ArgAction<LaunchArgs> parsed;
			try {
				parsed = Args.InvokeAction<LaunchArgs>(args);
			} catch (ArgException ex) {
				Console.WriteLine(ex.Message);
				Console.WriteLine(ArgUsage.GenerateUsageFromTemplate<LaunchArgs>());
				return;
			}
			if (parsed.Args == null) {
				return;
			}
		}
	}
}
