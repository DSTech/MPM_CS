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
using semver.tools;

namespace MPM.CLI {
    public class ListActionProvider {
        public void Provide(IContainer factory, ListArgs args) {
            this.List(factory);
        }

        public void List(IContainer factory) {
            var repository = factory.Resolve<IPackageRepository>();

            var packageList = repository.FetchPackageList();

            foreach (var _package in packageList) {
                var package = repository.FetchPackage(_package.Name);
                Console.WriteLine($"{package.Name} <{String.Join(", ", package.Authors)}>");
                foreach (var build in package.Builds) {
                    Console.WriteLine($"\t{(build.Arch)}#{build.Version} {build.GivenVersion}");
                }
            }
        }
    }
}
