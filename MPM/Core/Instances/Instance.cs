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
using System.IO;
using MPM.Data;
using System.Data;
using Community.CsharpSqlite.SQLiteClient;

namespace MPM.Core.Instances {
	public class Instance {
		const string MpmDirectory = ".mpm";
		const string DbDirectory = "db";
		const string DbName = "mpm.sqlite";
		const string ConfigurationName = "configuration";
		const string MetaName = "meta";
		const string PackageName = "packages";
		const string PackageCacheName = "packagecache";
		public String Location { get; set; }

		private IDbConnection CreateDbConnection() {
			SqliteConnection connection;
			{
				var dbPath = Path.Combine(Directory.CreateDirectory(Path.Combine(Location, MpmDirectory, DbDirectory)).FullName, DbName);
				if (!File.Exists(dbPath)) {
					File.WriteAllBytes(dbPath, new byte[0]);
				}
				var connStrBld = new SqliteConnectionStringBuilder() {
					DataSource = dbPath,
				};
				connection = new SqliteConnection(connStrBld.ConnectionString);
			}
			return connection;
		}
		public String Name {
			get {
				using (var meta = GetDbMeta()) {
					return meta.Get<String>("name");
				}
			}
			set {
				using (var meta = GetDbMeta()) {
					meta.Set<String>("name", value);
				}
			}
		}
		public Type LauncherType {
			get {
				using (var meta = GetDbMeta()) {
					return meta.Get<Type>("launcherType");
				}
			}
			set {
				using (var meta = GetDbMeta()) {
					meta.Set<Type>("launcherType", value);
				}
			}
		}

		public Instance() {
		}
		public Instance(String location) {
			this.Location = location;
		}

		public IMetaDataManager GetDbMeta() {
			return new DbMetaDataManager(CreateDbConnection(), MetaName);
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
