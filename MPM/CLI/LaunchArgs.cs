using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core;
using PowerArgs;
using semver.tools;

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
		[ArgShortcut("i"), ArgShortcut("init"), ArgShortcut("--init")]
		public void Init(InitArgs args) {
			Console.WriteLine("Init called, directory was " + args.InstancePath);
			if (!Directory.Exists(args.InstancePath)) {
				throw new IOException("Directory did not exist");
			}
			Console.WriteLine("Directory exists...");
			if (!args.ForceNonEmptyInstancePath && Directory.GetFiles(args.InstancePath).Length > 0) {
				throw new IOException("Directory was not empty; Use --force to override.");
			}
			Console.WriteLine("Creating instance...");
			var instanceArch = SemanticVersion.Parse(args.Arch);
			InstanceSide instanceSide;
			switch (args.Side) {
				case "client":
					instanceSide = InstanceSide.Client;
					break;
				case "server":
					instanceSide = InstanceSide.Server;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(instanceSide));
			}
			InstancePlatform instancePlatform;
			switch (args.Platform) {
				case "lin32":
					instancePlatform = InstancePlatform.Lin32;
					break;
				case "lin64":
					instancePlatform = InstancePlatform.Lin64;
					break;
				case "win32":
					instancePlatform = InstancePlatform.Win32;
					break;
				case "win64":
					instancePlatform = InstancePlatform.Win64;
					break;
				case "current": {
						var is64Bit = Environment.Is64BitOperatingSystem;
						var isLinux = Environment.OSVersion.Platform == PlatformID.Unix;
						if (isLinux) {
							instancePlatform = is64Bit ? InstancePlatform.Lin64 : InstancePlatform.Lin32;
						} else {
							instancePlatform = is64Bit ? InstancePlatform.Win64 : InstancePlatform.Win32;
						}
					} break;
				default:
					throw new ArgumentOutOfRangeException(nameof(instancePlatform));
			}
			var initActionProvider = new InitActionProvider();
			initActionProvider.Init(instanceArch, instanceSide, instancePlatform, args.InstancePath);
		}

		[ArgActionMethod]
		[ArgShortcut("l"), ArgShortcut("-l"), ArgShortcut("--launch")]
		public void LaunchMinecraft(LaunchMinecraftArgs args) {
			using (var minecraftLauncher = args.ToConfiguredLauncher()) {
				throw new NotImplementedException();//minecraftLauncher.Launch();
			}
		}
	}
}
