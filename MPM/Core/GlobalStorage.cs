using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using MPM.Core.Instances.Cache;
using MPM.Core.Profiles;
using MPM.Data;
using Newtonsoft.Json;

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
		private const string cacheName = "cache";
		private String HomePath => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		private LiteDatabase OpenGlobalDb() {
			var dbPath = Path.Combine(Directory.CreateDirectory(Path.Combine(HomePath, mpmDir)).FullName, $"{dbName}.litedb");
			return new LiteDatabase($"filename={dbPath}; journal=false");
		}

		public LiteDatabase FetchDataStore() => OpenGlobalDb();

		public IProfileManager FetchProfileManager() {
			return new LiteDbProfileManager(OpenGlobalDb(), profilesName);
		}

		public IMetaDataManager FetchMetaDataManager() {
			return new LiteDbMetaDataManager(OpenGlobalDb(), metaName);
		}

		public IProfile FetchProfile(Guid profileId) {
			using (var profileManager = FetchProfileManager()) {
				return profileManager.Fetch(profileId);
			}
		}

		public ICacheManager FetchGlobalCache() {
			var dbPath = Path.Combine(Directory.CreateDirectory(Path.Combine(HomePath, mpmDir, cacheName)).FullName, $"{dbName}_{cacheName}.litedb");
			var dbConnStr = $"filename={dbPath}; journal=false";
			return new LiteDbCacheManager(dbConnStr);
		}
	}
}
