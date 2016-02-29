using System;
using System.Linq;
using Autofac;
using MPM.Core;
using MPM.Core.Instances;
using MPM.Extensions;
using PowerArgs;

namespace MPM.CLI {
    public partial class RootArgs {
        [ArgActionMethod]
        [ArgShortcut("l"), ArgShortcut("-l"), ArgShortcut("launch"), ArgShortcut("--launch")]
        public void LaunchMinecraft(LaunchMinecraftArgs args) {
            var launchMinecraftActionProvider = new LaunchMinecraftActionProvider();
            launchMinecraftActionProvider.Provide(Resolver, args);
        }
    }

    public class LaunchMinecraftActionProvider : IActionProvider<LaunchMinecraftArgs> {
        public void Provide(IContainer resolver, LaunchMinecraftArgs args) {
            var global = resolver.Resolve<GlobalStorage>();
            var instance = new Instance(args.InstanceDirectory);
            var profile = global.FetchProfile(args.UserName);
            if (profile == null) {
                var firstProfileName = global.FetchProfileManager().Names.FirstOrDefault();
                if (firstProfileName == null) {
                    throw new Exception("No user found for launch. Please login.");
                }
                global.FetchProfile(firstProfileName);
            }
            using (var minecraftLauncher = args.ToConfiguredLauncher()) {
                minecraftLauncher.Launch(instance, profile);
            }
        }
    }
}
