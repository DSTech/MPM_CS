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
using semver.tools;

namespace MPM.CLI {
    public class CreatePackageActionProvider {
        public void Provide(IContainer factory, CreatePackageArgs args) {
            Console.WriteLine($"Creating package at path:\n\t{args.PackageDirectory}");
            throw new NotImplementedException();
        }
    }
}
