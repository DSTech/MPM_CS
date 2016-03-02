using System;
using System.Diagnostics;
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
            var profMgr = global.FetchProfileManager();
            var profile = profMgr.Fetch(args.UserName);
            if (profile == null) {
                var firstProfileName = profMgr.Names.FirstOrDefault();
                if (firstProfileName == null) {
                    throw new Exception("No user found for launch. Please login.");
                }
                profile = profMgr.Fetch(firstProfileName);
                Debug.Assert(profile != null, "If it found a name, that name should be valid");
            }
            using (var minecraftLauncher = args.ToConfiguredLauncher()) {
                minecraftLauncher.Launch(resolver, instance, profile);
            }
        }
    }
}
