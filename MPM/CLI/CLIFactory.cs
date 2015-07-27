using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using MPM.Core;
using MPM.Core.Dependency;
using MPM.Core.Instances.Cache;
using MPM.Core.Profiles;
using MPM.Data;

namespace MPM.CLI {

	public class CLIFactory {

		public IContainer GenerateResolver() {
			var cb = new ContainerBuilder();

			RegisterGlobalStorage(ref cb);
			RegisterGlobalMeta(ref cb);
			RegisterProfiles(ref cb);
			RegisterCache(ref cb);
			RegisterPackageRepository(ref cb);
			RegisterHashStore(ref cb);
			RegisterDependencyResolver(ref cb);//Consider auto-registering hashstores and packagerepo in an "IConfiguredResolver" interface?

			var container = cb.Build();
			Debug.Assert(container.IsRegistered<GlobalStorage>());
			Debug.Assert(container.IsRegistered<IUntypedKeyValueStore<String>>());
			Debug.Assert(container.IsRegistered<IMetaDataManager>());
			Debug.Assert(container.IsRegistered<IProfileManager>());
			Debug.Assert(container.IsRegistered<ICacheManager>());
			Debug.Assert(container.IsRegistered<IResolver>());
			Debug.Assert(container.IsRegistered<IPackageRepository>());
			Debug.Assert(container.IsRegistered<IHashRepository>());
			return container;
		}

		private void RegisterGlobalStorage(ref ContainerBuilder cb) {
			cb.Register<GlobalStorage>(ctxt => new GlobalStorage()).SingleInstance();
		}

		private void RegisterGlobalMeta(ref ContainerBuilder cb) {
			cb.Register<IUntypedKeyValueStore<String>>(ctxt => ctxt.Resolve<GlobalStorage>().FetchDataStore())
				.InstancePerDependency()
				.ExternallyOwned();
			cb.Register<IMetaDataManager>(ctxt => new DbMetaDataManager(ctxt.Resolve<GlobalStorage>().FetchDataStore()))
				.InstancePerDependency()
				.ExternallyOwned();
		}

		private void RegisterProfiles(ref ContainerBuilder cb) {
			cb.Register<IProfileManager>(ctxt => ctxt.Resolve<GlobalStorage>().FetchProfileManager()).SingleInstance();
		}

		private void RegisterCache(ref ContainerBuilder cb) {
			cb.Register<ICacheManager>(ctxt => ctxt.Resolve<GlobalStorage>().FetchGlobalCache()).SingleInstance();
		}

		private void RegisterDependencyResolver(ref ContainerBuilder cb) {
			cb.Register<IResolver>(ctxt => new Resolver()).SingleInstance();
		}

		private void RegisterPackageRepository(ref ContainerBuilder cb) {
			cb.Register<IPackageRepository>(ctxt => {
				using (var meta = ctxt.Resolve<IUntypedKeyValueStore<String>>()) {//Use to fetch custom package repositories
					var packageRepositoryUri = new Uri(meta.Get<String>("packageRepositoryUri") ?? "http://dst.dessix.net:8950/");
					return new HttpPackageRepository(packageRepositoryUri);
				}
			}).SingleInstance();
		}

		private void RegisterHashStore(ref ContainerBuilder cb) {
			cb.Register<IHashRepository>(ctxt => {
				using (var meta = ctxt.Resolve<IUntypedKeyValueStore<String>>()) {//Use to fetch custom hash repositories
					var hashStoreUri = new Uri(meta.Get<String>("hashStoreUri") ?? "http://dst.dessix.net:8951/");
					return new NaiveHttpHashRepository(hashStoreUri);
				}
			}).SingleInstance();
		}
	}
}
