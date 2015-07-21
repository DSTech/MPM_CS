using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using MPM.Core.Dependency;
using MPM.Core.Instances;
using MPM.Core.Instances.Cache;
using MPM.Core.Protocols;
using MPM.Data;
using MPM.Net.DTO;
using semver.tools;

namespace MPM.CLI {
	public class InitActionProvider {
		public void Provide(IContainer factory, InitArgs args) {
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
				case "current":
					{
						var is64Bit = Environment.Is64BitOperatingSystem;
						var isLinux = Environment.OSVersion.Platform == PlatformID.Unix;
						if (isLinux) {
							instancePlatform = is64Bit ? InstancePlatform.Lin64 : InstancePlatform.Lin32;
						} else {
							instancePlatform = is64Bit ? InstancePlatform.Win64 : InstancePlatform.Win32;
						}
					}
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(instancePlatform));
			}
			this.Init(factory, instanceArch, instanceSide, instancePlatform, args.InstancePath);
		}

		public Configuration GenerateArchConfiguration(SemanticVersion instanceArch, InstanceSide instanceSide, InstancePlatform instancePlatform) {
			PackageSide packageSide;
			switch (instanceSide) {
				case InstanceSide.Client:
					packageSide = PackageSide.Client;
					break;
				case InstanceSide.Server:
					packageSide = PackageSide.Server;
					break;
				default:
					throw new NotSupportedException();
			};
			return new Configuration() {
				Packages = new[] {
					new PackageSpec {
						Name = "minecraft",
						Arch = instanceArch.ToString(),
						Manual = true,
						Platform = instancePlatform.ToString(),
						Side = packageSide,
						Version = new VersionSpec(SemanticVersion.Parse("0.0.0"), true, SemanticVersion.Parse("9999.9999.9999"), true),
					},
				},
			};
		}

		public void Init(IContainer factory, SemanticVersion instanceArch, InstanceSide instanceSide, InstancePlatform instancePlatform, string instancePath) {
			var instance = new Instance(instancePath) {
				Name = $"{instanceArch}_{instanceSide}_{instancePlatform}",//TODO: make configurable and able to be immediately registered upon creation
				LauncherType = typeof(MinecraftLauncher),//TODO: make configurable via instanceSide, instanceArch and able to be overridden
				Configuration = InstanceConfiguration.Empty,
			};

			//TODO: Install arch pseudopackage
			var resolver = factory.Resolve<IResolver>();
			var repository = factory.Resolve<IPackageRepository>();
			
			Console.WriteLine("Generating configuration...");
			var archConfiguration = GenerateArchConfiguration(instanceArch, instanceSide, instancePlatform);

			Console.WriteLine("Attempting to resolve packages...");
			var resolvedArchConfiguration = resolver.Resolve(archConfiguration, repository);
			Console.WriteLine("Configuration resolved.");

			//var cacheManager = factory.Resolve<ICacheManager>();
			//var protocolResolver = factory.Resolve<IProtocolResolver>();
			throw new NotImplementedException();
		}
	}
}
