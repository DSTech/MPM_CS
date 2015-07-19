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
		[ArgShortcut(ArgShortcutPolicy.ShortcutsOnly), ArgShortcut("i"), ArgShortcut("--init")]
		public void Init(InitArgs args) {
			Console.WriteLine("Initializing instance at path:\n\t{0}", Path.GetFullPath(args.InstancePath));
			if (!Directory.Exists(args.InstancePath)) {
				Console.WriteLine("Directory did not exist. Creating...");
				Directory.CreateDirectory(args.InstancePath);
			} else {
				Console.WriteLine("Directory exists...");
				var dirEmpty = true;
				foreach (var fsEntry in Directory.EnumerateFileSystemEntries(args.InstancePath)) {
					dirEmpty = false;
					break;
				}
				if (!dirEmpty) {
					if (!args.ForceNonEmptyInstancePath) {
						Console.WriteLine("Directory was not empty; Use --force to override.");
						return;
					} else {
						Console.WriteLine("Directory contained files but --force was enabled. Overwriting...");
					}
				} else {
					Console.WriteLine("Directory was empty, proceeding...");
				}
			}
			Console.WriteLine("Creating instance...");
			SemanticVersion instanceArch;
			if (args.Arch == "latest") {
				instanceArch = SemanticVersion.Parse("1.8.4");
			} else {
				instanceArch = SemanticVersion.Parse(args.Arch);
			}
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
