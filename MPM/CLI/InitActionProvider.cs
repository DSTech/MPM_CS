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
            SemVer.Version instanceArch;
            if (args.Arch == "latest") {
                instanceArch = new SemVer.Version("1.8.8", true);
            } else {
                instanceArch = new SemVer.Version(args.Arch, true);
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

            this.Init(factory, instanceArch, instanceSide, args.InstancePath).WaitAndUnwrapException();
        }

        public Configuration GenerateArchConfiguration(SemVer.Version instanceArch, InstanceSide instanceSide) {
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
            }
            ;
            return new Configuration(new[] {
                new PackageSpec {
                    Name = "minecraft",
                    Arch = new Arch(instanceArch.ToString()),
                    Manual = true,
                    Side = packageSide,
                    VersionSpec = new SemVer.Range("*.*.*", true),
                },
            });
        }

        public async Task Init(IContainer factory, SemVer.Version instanceArch, InstanceSide instanceSide, string instancePath) {
            using (var instance = new Instance(instancePath) {
                Name = $"{instanceArch}_{instanceSide}",//TODO: make configurable and able to be immediately registered upon creation
                LauncherType = typeof(MinecraftLauncher),//TODO: make configurable via instanceSide, instanceArch and able to be overridden
                Configuration = InstanceConfiguration.Empty,
            }) {
                //TODO: Install arch pseudopackage
                var resolver = factory.Resolve<IDependencyResolver>();
                var repository = factory.Resolve<IPackageRepository>();

                Console.WriteLine("Generating configuration...");
                var archConfiguration = GenerateArchConfiguration(instanceArch, instanceSide);

                Console.WriteLine("Attempting to resolve packages...");
                var resolvedArchConfiguration = resolver.Resolve(archConfiguration, repository);
                Console.WriteLine("Configuration resolved.");

                var cacheManager = factory.Resolve<ICacheManager>();
                var hashRepository = factory.Resolve<IHashRepository>();

                instance.Configuration = resolvedArchConfiguration;

                foreach (var package in resolvedArchConfiguration.Packages) {
                    var cacheEntryName = $"package/{package.PackageName}_{package.Version}_{package.Arch}_{package.Side}";
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
