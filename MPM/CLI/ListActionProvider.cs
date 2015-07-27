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
using MPM.Net.DTO;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;
using semver.tools;

namespace MPM.CLI {
	public class ListActionProvider {
		public void Provide(IContainer factory, ListArgs args) {
			this.List(factory).WaitAndUnwrapException();
		}

		public async Task List(IContainer factory) {
			var repository = factory.Resolve<IPackageRepository>();

			var packageList = await repository.FetchPackageList();

			//var packageDetails = new List<Package>();

			foreach (var _package in packageList) {
				var package = repository.FetchPackage(_package.Name).WaitAndUnwrapException();
				Console.WriteLine($"{package.Name} <{String.Join(", ", package.Authors)}>");
				foreach (var build in package.Builds) {
					Console.WriteLine($"\t{(build.Arch)}#{build.Version} ({(build.Stable ? "STABLE" : "UNSTABLE")}): {build.GivenVersion}");
				}
			}
		}
	}
}
