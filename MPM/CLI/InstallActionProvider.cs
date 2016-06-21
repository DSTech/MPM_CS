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
using PowerArgs;

namespace MPM.CLI {
    public partial class RootArgs {
        [ArgActionMethod]
        [ArgShortcut("--install")]
        public void Install(InstallArgs args) {
            var initActionProvider = new InstallActionProvider();
            initActionProvider.Provide(Resolver, args);
        }
    }

    public class InstallActionProvider : IActionProvider<InstallArgs> {
        public void Provide(IContainer factory, InstallArgs args) {
            var instanceDir = args.InstanceDirectory;
            if (!instanceDir.Exists) {
                using (ConsoleColorZone.Error) {
                    Console.WriteLine("Directory did not exist. Creating...");
                }
                return;
            }
            this.Install(factory, args.InstanceDirectory, args.Packages);
        }

        private static Configuration GenerateArchConfiguration(MPM.Types.SemVersion instanceArch, CompatibilitySide instanceSide) {
            return new Configuration(new[] {
                new PackageSpec {
                    Name = "minecraft",
                    Arch = new Arch(instanceArch.ToString()),
                    Manual = true,
                    Side = instanceSide,
                    VersionSpec = new MPM.Types.SemRange("*.*.*", true),
                },
            });
        }

        public void Install(IContainer factory, DirectoryInfo instanceDirectory, IEnumerable<string> packages) {
            using (var instance = new Instance(instanceDirectory)) {
                var resolver = factory.Resolve<IDependencyResolver>();
                var repository = factory.Resolve<IPackageRepository>();

                Console.WriteLine("Generating configuration...");
                var unresolvedConfig = instance.BaseConfiguration;
                {
                    Console.WriteLine($"Contains {unresolvedConfig.Packages.Count} packages...");
                    // The installed arch package
                    var basePackage = instance.InstalledConfiguration.Packages.First();
                    foreach (var package in packages) {
                        var pSpec = new PackageSpec();
                        pSpec.Arch = basePackage.Arch;
                        pSpec.Manual = true;
                        pSpec.Name = package;
                        //TODO: Allow specification of desired versions
                        pSpec.VersionSpec = new SemRange("*", true);
                        unresolvedConfig.Packages.Add(pSpec);
                    }
                    Console.WriteLine($"Result contains {unresolvedConfig.Packages.Count} packages.");
                }

                Console.WriteLine("Attempting to resolve packages...");
                unresolvedConfig.Side = instance.Side;
                InstanceConfiguration resolvedConfiguration;
                try {
                    resolvedConfiguration = resolver.Resolve(unresolvedConfig, repository);
                } catch (DependencyException e) {
                    Huminz.DisplayException(e);
                    return;
                }
                Console.WriteLine("Configuration resolved.");

                var globalStorage = factory.Resolve<GlobalStorage>();
                var cacheManager = globalStorage.FetchGlobalCache();
                var hashRepository = factory.Resolve<IHashRepository>();
                var protocolResolver = factory.Resolve<IProtocolResolver>();

                var installer = new Installer(
                    instance,
                    repository,
                    hashRepository,
                    cacheManager,
                    protocolResolver
                    );

                installer.Install(resolvedConfiguration);
                // Save results only on success
                instance.BaseConfiguration = unresolvedConfig;
                instance.InstalledConfiguration = resolvedConfiguration;
            }
        }
    }
}
