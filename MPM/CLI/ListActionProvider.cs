using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using MPM.Core.Dependency;
using MPM.Core.Instances;
using MPM.Core.Instances.Cache;
using MPM.Core.Protocols;
using MPM.Data;
using MPM.Data.Repository;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;
using PowerArgs;

namespace MPM.CLI {
    public partial class RootArgs {
        [ArgActionMethod]
        [ArgShortcut(ArgShortcutPolicy.ShortcutsOnly), ArgShortcut("list"), ArgShortcut("--list")]
        public void ListPackages(ListArgs args) {
            var listActionProvider = new ListActionProvider();
            listActionProvider.Provide(Resolver, args);
        }
    }

    public class ListActionProvider : IActionProvider<ListArgs> {
        public void Provide(IContainer factory, ListArgs args) {
            this.List(factory);
        }

        public void List(IContainer factory) {
            var repository = factory.Resolve<IPackageRepository>();

            var packageList = repository.FetchBuilds()
                .OrderBy(b => b.PackageName)
                .ThenByDescending(b => b.Version)
                .GroupBy(b => b.PackageName)
                .ToArray();

            foreach (var _package in packageList) {
                var first = _package.First();
                Console.WriteLine($"{first.PackageName} <{String.Join(", ", first.Authors)}>");
                foreach (var build in _package) {
                    Console.WriteLine($"\tA:{(build.Arch)} #{build.Version} - \"{build.GivenVersion}\"");
                }
            }
        }
    }
}
