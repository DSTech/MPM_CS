using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Cache;
using MPM.Data;

namespace MPM.Core {
	/// <summary>
	/// Should:
	///	 Provide:
	///	  Root Meta Database: <see cref="IUntypedKeyValueStore{String}"/>
	///	  Profile Store: <see cref="Func{Guid, IProfile}"/>
	///   Global Cache: <see cref="Func{ICacheManager}"/>
	/// </summary>
	public class GlobalStorage {
		const string mpmDir = ".mpm";
		private String HomePath => (
			Environment.OSVersion.Platform == PlatformID.Unix ||
			Environment.OSVersion.Platform == PlatformID.MacOSX
			) ?
				Environment.GetEnvironmentVariable("HOME") :
				Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
		public IUntypedKeyValueStore<String> FetchDataStore() {
			SQLiteConnection connection;
			{
				var dbPath = Path.Combine(Directory.CreateDirectory(Path.Combine(HomePath, mpmDir)).FullName, "global.sqlite");
				if (!File.Exists(dbPath)) {
					SQLiteConnection.CreateFile(dbPath);
				}
				var connStrBld = new SQLiteConnectionStringBuilder() {
					DataSource = dbPath,
				};
				connection = new SQLiteConnection(connStrBld.ConnectionString);
			}
			return new DbKeyValueStore<String>(connection, "meta");
		}
		public IProfile FetchProfile(Guid profileId) {
			throw new NotImplementedException();
		}
		public ICacheManager FetchGlobalCache() {
			var cachePath = Directory.CreateDirectory(Path.Combine(HomePath, mpmDir, "cache")).FullName;
			var cacheManager = new FileSystemCacheManager(cachePath);
			return cacheManager;
		}
	}
}
