using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using MPM.ActionProviders;
using MPM.Core;
using MPM.Core.Dependency;
using MPM.Core.Instances;
using MPM.Core.Instances.Cache;
using MPM.Core.Instances.Installation;
using MPM.Core.Protocols;
using MPM.Data;
using MPM.Data.Repository;
using MPM.Types;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;
using semver.tools;

namespace MPM.CLI {

	public class InitActionProvider {

		public void Provide(IContainer factory, InitArgs args) {
			Console.WriteLine($"Initializing instance at path:\n\t{Path.GetFullPath(args.InstancePath)}");
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
				instanceArch = SemanticVersion.Parse("1.8.8");
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
					}
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(instancePlatform));
			}
			this.Init(factory, instanceArch, instanceSide, instancePlatform, args.InstancePath).WaitAndUnwrapException();
		}

		public CompatibilityPlatform ConvertToCompatibility(InstancePlatform instancePlatform) {
			switch (instancePlatform) {
				case InstancePlatform.Lin32: return CompatibilityPlatform.Lin32;
				case InstancePlatform.Lin64: return CompatibilityPlatform.Lin64;
				case InstancePlatform.Win32: return CompatibilityPlatform.Win32;
				case InstancePlatform.Win64: return CompatibilityPlatform.Win64;
				default: return CompatibilityPlatform.Universal;
			}
		}

		public Configuration GenerateArchConfiguration(SemanticVersion instanceArch, InstanceSide instanceSide, InstancePlatform instancePlatform) {
			CompatibilitySide packageSide;
			switch (instanceSide) {
				case InstanceSide.Client:
					packageSide = CompatibilitySide.Client;
					break;
				case InstanceSide.Server:
					packageSide = CompatibilitySide.Server;
					break;
				default:
					throw new NotSupportedException();
			};
			return new Configuration(new[] {
				new PackageSpec {
					Name = "minecraft",
					Arch = new Arch(instanceArch.ToString()),
					Manual = true,
					Platform = ConvertToCompatibility(instancePlatform),
					Side = packageSide,
					VersionSpec = new VersionSpec(SemanticVersion.Parse("0.0.0"), true, SemanticVersion.Parse("9999.9999.9999"), true),
				},
			});
		}

		public async Task Init(IContainer factory, SemanticVersion instanceArch, InstanceSide instanceSide, InstancePlatform instancePlatform, string instancePath) {
			using (var instance = new Instance(instancePath) {
				Name = $"{instanceArch}_{instanceSide}_{instancePlatform}",//TODO: make configurable and able to be immediately registered upon creation
				LauncherType = typeof(MinecraftLauncher),//TODO: make configurable via instanceSide, instanceArch and able to be overridden
				Configuration = InstanceConfiguration.Empty,
			}) {

				//TODO: Install arch pseudopackage
				var resolver = factory.Resolve<IDependencyResolver>();
				var repository = factory.Resolve<IPackageRepository>();

				Console.WriteLine("Generating configuration...");
				var archConfiguration = GenerateArchConfiguration(instanceArch, instanceSide, instancePlatform);

				Console.WriteLine("Attempting to resolve packages...");
				var resolvedArchConfiguration = resolver.Resolve(archConfiguration, repository);
				Console.WriteLine("Configuration resolved.");

				var cacheManager = factory.Resolve<ICacheManager>();
				var hashRepository = factory.Resolve<IHashRepository>();

				instance.Configuration = resolvedArchConfiguration;

				foreach (var package in resolvedArchConfiguration.Packages) {
					var cacheEntryName = $"package/{package.PackageName}_{package.Version}_{package.Arch}_{package.Side}_{package.Platform}";
					if (cacheManager.Contains(cacheEntryName)) {
						Console.WriteLine($"Package {package.PackageName} already cached.");
						continue;
					}
					Console.WriteLine($"Downloading package {package.PackageName} to cache...");
					var packageArchive = await hashRepository.RetrieveArchive(package.PackageName, package.Hashes);
					cacheManager.Store(cacheEntryName, packageArchive);
				}

				var installer = new Installer(
					instance,
					repository,
					factory.Resolve<IHashRepository>(),
					cacheManager,
					factory.Resolve<IProtocolResolver>()
				);

				await installer.Install(resolvedArchConfiguration);

				//var cacheManager = factory.Resolve<ICacheManager>();
				//var protocolResolver = factory.Resolve<IProtocolResolver>();
				throw new NotImplementedException();
			}
		}
	}
}
