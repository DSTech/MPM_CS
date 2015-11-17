using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Disposables;
using LiteDB;
using MPM.Core.Instances.Cache;
using MPM.Core.Profiles;
using MPM.Data;
using Newtonsoft.Json;
using Autofac;

namespace MPM.Core {

	/// <summary>
	/// Should:
	///	 Provide:
	///	  Root Meta Database: <see cref="IUntypedKeyValueStore{String}"/>
	///	  Profile Store: <see cref="Func{Guid, IProfile}"/>
	///   Global Cache: <see cref="Func{ICacheManager}"/>
	/// </summary>
	public class GlobalStorage : ICancelable {

		private const string CacheName = "cache";
		private const string DbName = "global";
		private const string MetaName = "meta";
		private const string MpmDirName = ".mpm";
		private const string ProfilesName = "profiles";

		public GlobalStorage() {
			var cb = new ContainerBuilder();

			cb.Register<LiteDatabase>(ctxt => new LiteDatabase($"filename={DbPath}; journal=false"))
				.AsSelf()
				.SingleInstance()
				.Named<LiteDatabase>("GlobalDb");

			cb.Register<ICacheManager>(ctxt => new FileSystemCacheManager(CachePath))
				.As<ICacheManager>()
				.SingleInstance()
				.Named<ICacheManager>("GlobalCache");

			cb.Register<IProfileManager>(ctxt => new LiteDbProfileManager(ctxt.Resolve<LiteDatabase>().GetCollection(ProfilesName)))
				.As<IProfileManager>()
				.SingleInstance()
				.Named<IProfileManager>("GlobalProfiles");

			cb.Register<IMetaDataManager>(ctxt => new LiteDbMetaDataManager(ctxt.Resolve<LiteDatabase>().GetCollection(MetaName)))
				.As<IMetaDataManager>()
				.SingleInstance()
				.Named<IMetaDataManager>("GlobalMetaData");

			this.Factory = cb.Build();
		}

		public string CachePath => Directory.CreateDirectory(Path.Combine(HomePath, CacheName)).FullName;

		public string DbPath => Path.Combine(HomePath, $"{DbName}.litedb");

		public bool IsDisposed { get; private set; }
		private IContainer Factory { get; set; }

		private String HomePath =>
			Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), MpmDirName)).FullName;

		public void Dispose() {
			Dispose(true);
		}

		public LiteDatabase FetchDataStore() => Factory.Resolve<LiteDatabase>();

		public ICacheManager FetchGlobalCache() => Factory.Resolve<ICacheManager>();

		public IMetaDataManager FetchMetaDataManager() => Factory.Resolve<IMetaDataManager>();

		public IProfile FetchProfile(Guid profileId) => FetchProfileManager().Fetch(profileId);

		public IProfileManager FetchProfileManager() => Factory.Resolve<IProfileManager>();

		protected virtual void Dispose(bool disposing) {
			if (IsDisposed) {
				return;
			}
			Factory.Dispose();
			GC.SuppressFinalize(this);
			IsDisposed = true;
		}

	}
}
