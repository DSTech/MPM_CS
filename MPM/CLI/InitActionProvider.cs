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
using MPM.Util;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;

namespace MPM.CLI {
    public class InitActionProvider {
        public void Provide(IContainer factory, InitArgs args) {
            var instanceDir = args.InstanceDirectory;
            using (ConsoleColorZone.Success)
                Console.WriteLine($"Initializing instance at:\n\t{instanceDir.FullName}");
            if (!instanceDir.Exists) {
                Console.WriteLine("Directory did not exist. Creating...");
                instanceDir.Create();
            } else {
                using (ConsoleColorZone.Info) Console.WriteLine("Directory exists...");
                var dirEmpty = instanceDir.EnumerateFileSystemInfos().IsEmpty();
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
                var resolver = factory.Resolve<IDependencyResolver>();
                var repository = factory.Resolve<IPackageRepository>();

                Console.WriteLine("Generating configuration...");
                var archConfiguration = GenerateArchConfiguration(instanceArch, instanceSide);

                Console.WriteLine("Attempting to resolve packages...");
                InstanceConfiguration resolvedArchConfiguration;
                try {
                    resolvedArchConfiguration = resolver.Resolve(archConfiguration, repository);
                } catch (DependencyException e) {
                    using (ConsoleColorZone.Error) {
                        var inner = e.InnerException;
                        if (inner != null) {
                            Console.WriteLine("Error: {0}", inner.Message);
                            var depinner = inner as DependencyException;
                            if (depinner != null) {
                                Console.WriteLine("Details:");
                                Console.WriteLine("\t{0}", depinner.PackageSpec);
                            }
                        } else {
                            Console.WriteLine(e.Message);
                        }
                    }
                    return;
                }
                Console.WriteLine("Configuration resolved.");

                var globalStorage = factory.Resolve<GlobalStorage>();
                var cacheManager = globalStorage.FetchGlobalCache();
                var hashRepository = factory.Resolve<IHashRepository>();
                var protocolResolver = factory.Resolve<IProtocolResolver>();

                instance.Configuration = resolvedArchConfiguration;
                
                var installer = new Installer(
                    instance,
                    repository,
                    hashRepository,
                    cacheManager,
                    protocolResolver
                    );

                installer.Install(resolvedArchConfiguration);
            }
        }
    }
}
