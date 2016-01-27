using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using MPM.Core;
using MPM.Extensions;
using PowerArgs;

namespace MPM.CLI {
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public class LaunchArgs {
        [ArgIgnore]
        public IContainer Resolver { get; set; }

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
            Console.WriteLine($"Current version is {manager.Version}");
            foreach (var update in manager.FindUpdates()) {
                Console.WriteLine(update);
            }
        }

        [ArgActionMethod]
        [ArgShortcut(ArgShortcutPolicy.ShortcutsOnly), ArgShortcut("i"), ArgShortcut("--init")]
        public void Init(InitArgs args) {
            var initActionProvider = new InitActionProvider();
            initActionProvider.Provide(Resolver, args);
        }

        [ArgActionMethod]
        [ArgShortcut(ArgShortcutPolicy.ShortcutsOnly), ArgShortcut("list"), ArgShortcut("--list")]
        public void ListPackages(ListArgs args) {
            var listActionProvider = new ListActionProvider();
            listActionProvider.Provide(Resolver, args);
        }

        [ArgActionMethod]
        [ArgShortcut(ArgShortcutPolicy.ShortcutsOnly), ArgShortcut("pack"), ArgShortcut("create"), ArgShortcut("build")]
        public void CreatePackage(CreatePackageArgs args) {
            var createPackageActionProvider = new CreatePackageActionProvider();
            createPackageActionProvider.Provide(Resolver, args);
        }

        /*[ArgActionMethod]
		[ArgShortcut(ArgShortcutPolicy.ShortcutsOnly), ArgShortcut("s"), ArgShortcut("--sync")]
		public void Sync(InitArgs args) {
			var syncActionProvider = new SyncActionProvider();
			syncActionProvider.Provide(Resolver, args);
		}*/

        [ArgActionMethod]
        [ArgShortcut("l"), ArgShortcut("-l"), ArgShortcut("--launch")]
        public void LaunchMinecraft(LaunchMinecraftArgs args) {
            using (var minecraftLauncher = args.ToConfiguredLauncher()) {
                throw new NotImplementedException();//minecraftLauncher.Launch();
            }
        }
    }
}
