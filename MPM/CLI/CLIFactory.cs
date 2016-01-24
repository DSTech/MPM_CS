using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using LiteDB;
using MPM.Core;
using MPM.Core.Dependency;
using MPM.Core.Instances.Cache;
using MPM.Core.Profiles;
using MPM.Core.Protocols;
using MPM.Data;
using MPM.Data.Repository;
using MPM.Net;
using MPM.Net.Protocols.Minecraft;
using MPM.Types;

namespace MPM.CLI {

	public class CLIFactory {

		public IContainer GenerateResolver() {
			var cb = new ContainerBuilder();

			RegisterGlobalStorage(ref cb);
			RegisterPackageRepository(ref cb);
			RegisterHashStore(ref cb);
			RegisterDependencyResolver(ref cb);//Consider auto-registering hashstores and packagerepo in an "IConfiguredResolver" interface?
			RegisterProtocolResolver(ref cb);

			var container = cb.Build();
			Debug.Assert(container.IsRegistered<GlobalStorage>());
			Debug.Assert(container.IsRegistered<IDependencyResolver>());
			Debug.Assert(container.IsRegistered<IPackageRepository>());
			Debug.Assert(container.IsRegistered<IHashRepository>());
			Debug.Assert(container.IsRegistered<IProtocolResolver>());
			return container;
		}

		private void RegisterGlobalStorage(ref ContainerBuilder cb) {
			cb.Register<GlobalStorage>(ctxt => new GlobalStorage()).SingleInstance();
		}

		private void RegisterPackageRepository(ref ContainerBuilder cb) {
			cb.Register<IPackageRepository>(ctxt => {
				var meta = ctxt.Resolve<GlobalStorage>().FetchMetaDataManager();//Use to fetch custom package repositories
				var packageRepositoryUri = new Uri(meta.Get<String>("packageRepositoryUri") ?? "http://dst.dessix.net:8950/");
				return new HttpPackageRepository(packageRepositoryUri);
			}).SingleInstance();
		}

		private void RegisterHashStore(ref ContainerBuilder cb) {
			cb.Register<IHashRepository>(ctxt => {
				var meta = ctxt.Resolve<GlobalStorage>().FetchMetaDataManager();//Use to fetch custom hash repositories
				var hashStoreUri = new Uri(meta.Get<String>("hashStoreUri") ?? "http://dst.dessix.net:8951/");
				return new NaiveHttpHashRepository(hashStoreUri);
			}).SingleInstance();
		}

		private void RegisterDependencyResolver(ref ContainerBuilder cb) {
			cb.Register<IDependencyResolver>(ctxt => new DependencyResolver()).SingleInstance();
		}

		private void RegisterProtocolResolver(ref ContainerBuilder cb) {
			cb.Register<IProtocolResolver>(ctxt => new CLIProtocolResolver(ctxt.Resolve<IHashRepository>())).SingleInstance();
		}
	}
}
