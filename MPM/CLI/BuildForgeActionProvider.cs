using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using ICSharpCode.SharpZipLib.Zip;
using MPM.Core.Instances.Installation.Scripts;
using MPM.Types;
using Newtonsoft.Json;
using MPM.Util;
using PowerArgs;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;
using Alphaleonis.Win32.Filesystem;

namespace MPM.CLI {
    public partial class RootArgs {
        [ArgActionMethod]
        [ArgShortcut(ArgShortcutPolicy.ShortcutsOnly), ArgShortcut("pack"), ArgShortcut("create"), ArgShortcut("build")]
        public void BuildForge(BuildForgeArgs args) {
            var createPackageActionProvider = new BuildForgeActionProvider();
            createPackageActionProvider.Provide(Resolver, args);
        }
    }

    public class BuildForgeActionProvider : IActionProvider<BuildForgeArgs> {
        public void Provide(IContainer factory, BuildForgeArgs args) {
            if (!args.PackageDirectory.Exists) { throw new DirectoryNotFoundException(); }
            if (!args.PackageSpecFile.Exists) { throw new FileNotFoundException(); }
            Console.WriteLine($"Creating forge package at path:\n\t{args.PackageDirectory}");
            Environment.CurrentDirectory = args.PackageDirectory.FullName;

            throw new NotImplementedException();//TODO: Build forge package from specified version, calling CreatePackage afterward
        }
    }
}
