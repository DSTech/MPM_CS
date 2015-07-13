using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Cache;
using MPM.Core.Profiles;
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
		const string dbName = "global.sqlite";
		const string metaName = "meta";
		const string profilesName = "profiles";
		private String HomePath => (
			Environment.OSVersion.Platform == PlatformID.Unix ||
			Environment.OSVersion.Platform == PlatformID.MacOSX
			) ?
				Environment.GetEnvironmentVariable("HOME") :
				Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
		private IDbConnection OpenGlobalDb() {
			SQLiteConnection connection;
			{
				var dbPath = Path.Combine(Directory.CreateDirectory(Path.Combine(HomePath, mpmDir)).FullName, dbName);
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
		public IUntypedKeyValueStore<String> FetchDataStore() {
			return new DbKeyValueStore<String>(OpenGlobalDb(), metaName);
		}
		public IProfileManager FetchProfileManager() {
			return new KeyValueStoreProfileManager(
				new DbKeyValueStore<Guid>(OpenGlobalDb(), profilesName)
					.Typify().As<IProfile>()
			);
		}
		public IProfile FetchProfile(Guid profileId) {
			using (var profileManager = FetchProfileManager()) {
				return profileManager.Fetch(profileId);
			}
		}
		public ICacheManager FetchGlobalCache() {
			var cachePath = Directory.CreateDirectory(Path.Combine(HomePath, mpmDir, "cache")).FullName;
			var cacheManager = new FileSystemCacheManager(cachePath);
			return cacheManager;
		}
	}
}
