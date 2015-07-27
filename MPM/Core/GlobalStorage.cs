using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Community.CsharpSqlite.SQLiteClient;
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
		private const string mpmDir = ".mpm";
		private const string dbName = "global.sqlite";
		private const string metaName = "meta";
		private const string profilesName = "profiles";
		private String HomePath => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

		private IDbConnection OpenGlobalDb() {
			SqliteConnection connection;
			{
				var dbPath = Path.Combine(Directory.CreateDirectory(Path.Combine(HomePath, mpmDir)).FullName, dbName);
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
