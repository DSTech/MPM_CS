using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core;
using PowerArgs;

namespace MPM.CLI {

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
		[ArgShortcut("uc"), ArgShortcut("--uc"), ArgShortcut("--updatecauldron")]
		public void UpdateCauldron(UpdateCauldronArgs args) {
			Console.WriteLine("UpdateCauldron called, file was " + args.CauldronFile);
		}

		[ArgActionMethod]
		[ArgShortcut("uf"), ArgShortcut("--uf"), ArgShortcut("--updateforge")]
		public void UpdateForge(UpdateForgeArgs args) {
			Console.WriteLine("UpdateForge called, directory was " + args.ForgeDirectory);
			IServerManager manager = new ForgeServerManager(args.ForgeDirectory);
			Console.WriteLine("Current version is {0}", manager.Version);
			foreach (var update in manager.FindUpdates()) {
				Console.WriteLine(update);
			}
		}

		[ArgActionMethod]
		[ArgShortcut("l"), ArgShortcut("-l"), ArgShortcut("--launch")]
		public void LaunchMinecraft(LaunchMinecraftArgs args) {
			using (var minecraftLauncher = args.ToConfiguredLauncher()) {
				minecraftLauncher.Launch();
			}
		}
	}
}
