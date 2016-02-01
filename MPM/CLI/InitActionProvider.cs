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
            var instanceDir = args.InstanceDirectory;
            Console.WriteLine($"Initializing instance at:\n\t{instanceDir}");
            if (!instanceDir.Exists) {
                Console.WriteLine("Directory did not exist. Creating...");
                instanceDir.Create();
            } else {
                Console.WriteLine("Directory exists...");
                var dirEmpty = true;
                foreach (var fsEntry in instanceDir.EnumerateFileSystemInfos()) {
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
            this.Init(factory, args.Arch, args.Side, args.InstanceDirectory);
        }

        private static Configuration GenerateArchConfiguration(MPM.Types.SemVersion instanceArch, InstanceSide instanceSide) {
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
            return new Configuration(new[] {
                new PackageSpec {
                    Name = "minecraft",
                    Arch = new Arch(instanceArch.ToString()),
                    Manual = true,
                    Side = packageSide,
                    VersionSpec = new MPM.Types.SemRange("*.*.*", true),
                },
            });
        }

        public void Init(IContainer factory, MPM.Types.SemVersion instanceArch, InstanceSide instanceSide, DirectoryInfo instanceDirectory) {
            using (var instance = new Instance(instanceDirectory) {
                Name = $"{instanceArch}_{instanceSide}",//TODO: make configurable and able to be immediately registered upon creation
                LauncherType = typeof(MinecraftLauncher),//TODO: make configurable via instanceSide/instanceArch and able to be overridden
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
                    var packageArchive = hashRepository.RetrieveArchive(package.PackageName, package.Hashes).WaitAndUnwrapException();
                    cacheManager.Store(cacheEntryName, packageArchive);
                }

                //TODO: Reevaluate this

                var installer = new Installer(
                    instance,
                    repository,
                    factory.Resolve<IHashRepository>(),
                    cacheManager,
                    factory.Resolve<IProtocolResolver>()
                    );

                installer.Install(resolvedArchConfiguration);

                //var cacheManager = factory.Resolve<ICacheManager>();
                //var protocolResolver = factory.Resolve<IProtocolResolver>();
                throw new NotImplementedException();
            }
        }
    }
}
