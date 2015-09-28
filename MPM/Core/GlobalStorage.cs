using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Cache;
using MPM.Core.Profiles;
using MPM.Data;
using Newtonsoft.Json;
using RaptorDB;

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
		private const string dbName = "global";
		private const string metaName = "meta";
		private const string profilesName = "profiles";
		private String HomePath => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		private KeyStore<T> OpenGlobalDb<T>(string dbTableName) where T : IComparable<T> {
			var dbPath = Path.Combine(Directory.CreateDirectory(Path.Combine(HomePath, mpmDir)).FullName, dbName);
			var db = new RaptorDB.RaptorDB<T>(Path.Combine(dbPath, $"{dbTableName}.rptr"), false);
			return db;
		}
		private KeyStore<string> OpenGlobalDb(string dbTableName) => OpenGlobalDb<string>(dbTableName);

		public IUntypedKeyValueStore<String> FetchDataStore() => new RaptorUntypedKeyValueStore<String>(OpenGlobalDb(metaName));

		public IProfileManager FetchProfileManager() {
			return new KeyValueStoreProfileManager(
				new RaptorUntypedKeyValueStore<Guid>(OpenGlobalDb<Guid>(profilesName)).Typify().As<IProfile>()
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
