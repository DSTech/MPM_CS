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
using System.Data.SQLite;

namespace MPM.Core.Instances {
	public class Instance {
		const string MpmDirectory = ".mpm";
		const string DbDirectory = "db";
		const string DbName = "mpm.sqlite";
		const string ConfigurationName = "configuration";
		const string MetaName = "meta";
		const string PackageName = "packages";
		const string PackageCacheName = "packagecache";

		private IDbConnection CreateDbConnection() {
			SQLiteConnection connection;
			{
				var dbPath = Path.Combine(Directory.CreateDirectory(Path.Combine(Location, MpmDirectory, DbDirectory)).FullName, DbName);
				if (!File.Exists(dbPath)) {
					SQLiteConnection.CreateFile(dbPath);
				}
				var connStrBld = new SQLiteConnectionStringBuilder() {
					DataSource = dbPath,
				};
				connection = new SQLiteConnection(connStrBld.ConnectionString);
			}
			return connection;
		}
		public String Name { get; set; }
		public String Location { get; set; }
		public Type LauncherType { get; set; } = typeof(MinecraftLauncher);//TODO: Change to a default (ScriptLauncher / ShellLauncher?), or auto-identify launch method

		public Instance() {
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
