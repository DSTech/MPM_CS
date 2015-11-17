using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using LiteDB;
using MPM.Core.Dependency;
using MPM.Core.Instances.Installation;
using MPM.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Platform.VirtualFileSystem;
using Platform.VirtualFileSystem.Providers.Local;

namespace MPM.Core.Instances {

	public class Instance : ICancelable {
		private const string MpmDirectoryName = ".mpm";
		private const string DbDirectory = "db";
		private const string DbName = "db";
		private const string ConfigurationName = "configuration";
		private const string MetaName = "meta";
		private const string PackageName = "packages";
		private const string PackageCacheName = "packagecache";
		public IContainer Factory { get; private set; }
		public String Location { get; set; }
		public String MpmDirectory => Directory.CreateDirectory(Path.Combine(Location, MpmDirectoryName)).FullName;
		public String DbPath => Path.Combine(MpmDirectory, $"{DbName}.litedb");

		public Instance(String location) {
			this.Location = location;
			var cb = new ContainerBuilder();

			cb.Register<LiteDatabase>(ctxt => new LiteDatabase($"filename={DbPath}; journal=false"))
				.AsSelf()
				.SingleInstance()
				.Named<LiteDatabase>("InstanceDb");

			cb.Register<IFileSystem>(ctxt => FileSystemManager.Default.ResolveDirectory(Location).CreateView())
				.As<IFileSystem>()
				.SingleInstance()
				.Named<IFileSystem>("InstanceProfiles");

			cb.Register<IMetaDataManager>(ctxt => new LiteDbMetaDataManager(ctxt.Resolve<LiteDatabase>().GetCollection(MetaName)))
				.As<IMetaDataManager>()
				.SingleInstance()
				.Named<IMetaDataManager>("InstanceMetaData");

			this.Factory = cb.Build();
		}

		private LiteDatabase FetchDbConnection() {
			return Factory.Resolve<LiteDatabase>();
		}

		public IMetaDataManager FetchDbMeta() {
			return Factory.Resolve<IMetaDataManager>();
		}

		public IFileSystem FetchFileSystem() {
			return Factory.Resolve<IFileSystem>();
		}

		public String Name {
			get {
				return FetchDbMeta().Get<String>("name");
			}
			set {
				FetchDbMeta().Set<String>("name", value);
			}
		}

		public Type LauncherType {
			get {
				return FetchDbMeta().Get<Type>("launcherType");
			}
			set {
				FetchDbMeta().Set<Type>("launcherType", value);
			}
		}

		public InstanceConfiguration @Configuration {
			get {
				var conf = FetchDbMeta().Get<InstanceConfiguration>(ConfigurationName);
				return conf ?? InstanceConfiguration.Empty;
			}
			set {
				FetchDbMeta().Set<InstanceConfiguration>(ConfigurationName, value);
			}
		}

		public ILauncher CreateLauncher() {
			var ctor = LauncherType.GetConstructor(new Type[0]);
			return (ILauncher)ctor.Invoke(new object[0]);
		}

		public bool IsDisposed { get; private set; }

		public void Dispose() {
			Dispose(true);
		}

		protected void Dispose(bool disposing) {
			if (IsDisposed) {
				return;
			}
			Factory.Dispose();
			GC.SuppressFinalize(this);
			IsDisposed = true;
		}
	}
}
