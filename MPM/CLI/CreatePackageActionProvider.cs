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
using Newtonsoft.Json;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;

namespace MPM.CLI {
    public class CreatePackageActionProvider {
        public void Provide(IContainer factory, CreatePackageArgs args) {
            if (!args.PackageDirectory.Exists) { throw new DirectoryNotFoundException(); }
            if (!args.PackageSpecFile.Exists) { throw new FileNotFoundException(); }
            Console.WriteLine($"Creating package at path:\n\t{args.PackageDirectory}");
            Environment.CurrentDirectory = args.PackageDirectory.FullName;

            var build = JsonConvert.DeserializeObject<Build>(File.ReadAllText(args.PackageSpecFile.FullName));

            Console.WriteLine(build);
            Console.WriteLine(JsonConvert.SerializeObject(build, Formatting.Indented));

            throw new NotImplementedException();
        }
    }
}
