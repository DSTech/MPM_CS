using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core;
using PowerArgs;

namespace MPM.CLI {
	public static class ActionProviderArgExtensions {
		public static MinecraftLauncher ToConfiguredLauncher(this LaunchMinecraftArgs self) {
			return new MinecraftLauncher {
				UserName = self.UserName,
			};
		}
	}
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
			foreach(var update in manager.FindUpdates()) {
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
	public class UpdateCauldronArgs {
		[ArgRequired(PromptIfMissing = true)]
		[ArgDescription("The Cauldron installer file to update")]
		[ArgPosition(1)]
		[ArgExistingFile]
		public string CauldronFile { get; set; }
	}
	public class UpdateForgeArgs {
		[ArgRequired(PromptIfMissing = true)]
		[ArgDescription("The Forge Directory to update")]
		[ArgPosition(1)]
		[ArgExistingDirectory]
		[DefaultValue(".")]
		public string ForgeDirectory { get; set; }
	}
	public class LaunchMinecraftArgs {
		[ArgDescription("The username of the profile to use")]
		[ArgShortcut("-u"), ArgShortcut("--user"), ArgShortcut("--username")]
		[ArgEnforceCase]
		public string UserName { get; set; }
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
