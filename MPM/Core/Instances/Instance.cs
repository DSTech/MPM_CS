using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Dependency;
using MPM.Core.Instances.Installation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Platform.VirtualFileSystem;
using Platform.VirtualFileSystem.Providers.Local;
using Couchbase.Lite;
using System.IO;
using MPM.Data;

namespace MPM.Core.Instances {
	public class Instance {
		const string MpmDirectory = ".mpm";
		const string DbDirectory = "db";
		const string ConfigurationName = "configuration";
		const string MetaName = "meta";
		const string PackageName = "packages";
		const string PackageCacheName = "packagecache";

		private Manager DbManager { get; set; }
		public String Name { get; set; }
		private String _location = null;
		public String Location {
			get {
				return _location;
			}
			set {
				_location = value;
				if (_location != null) {
					DbManager = new Manager(Directory.CreateDirectory(Path.Combine(_location, MpmDirectory, DbDirectory)), ManagerOptions.Default);
				}
			}
		}
		public Type LauncherType { get; set; } = typeof(MinecraftLauncher);//TODO: Change to a default (ScriptLauncher / ShellLauncher?), or auto-identify launch method

		public Instance() {
		}

		private Database GetDb(string databaseName) {
			return DbManager.GetDatabase(databaseName);
		}

		public IMetaDataManager GetDbMeta() {
			return new CouchbaseMetaDataManager(GetDb(MetaName));
		}

		public IFileSystem GetFileSystem() {
			return FileSystemManager.Default.ResolveDirectory(Location).CreateView();
		}

		public InstanceConfiguration @Configuration {
			get {
				using (var meta = GetDbMeta()) {
					var conf = meta.Get<InstanceConfiguration>(ConfigurationName);
					return conf ?? InstanceConfiguration.Empty;
				}
			}
			set {
				using (var meta = GetDbMeta()) {
					meta.Set<InstanceConfiguration>(ConfigurationName, value);
				}
			}
		}

		public ILauncher CreateLauncher() {
			var ctor = LauncherType.GetConstructor(new Type[0]);
			return (ILauncher)ctor.Invoke(new object[0]);
		}
	}
}
