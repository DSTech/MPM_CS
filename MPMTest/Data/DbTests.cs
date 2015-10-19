using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Newtonsoft.Json;
using MPM.Extensions;
using LiteDB;
using MPM.Data;
using MPM.Core.Profiles;
using System.IO;
using System.Threading;

namespace MPMTest.Core {

	public class DbTests {
		private readonly ITestOutputHelper output;

		public DbTests(ITestOutputHelper output) {
			this.output = output;
		}

		public class DbTestPair {
			public static DbTestPair<TK, TV> Create<TK, TV>(TK key, TV value) => new DbTestPair<TK, TV>(key, value);
		}

		public class DbTestPair<TK, TV> {
			public TK Key { get; set; }
			public TV Value { get; set; }
			public DbTestPair() { }
			public DbTestPair(TK key, TV value) {
				this.Key = key;
				this.Value = value;
			}
			public static implicit operator KeyValuePair<TK, TV>(DbTestPair<TK, TV> pair) => new KeyValuePair<TK, TV>(pair.Key, pair.Value);
			public static implicit operator DbTestPair<TK, TV>(KeyValuePair<TK, TV> pair) => new DbTestPair<TK, TV>(pair.Key, pair.Value);
		}

		[Fact]
		public void Enumeration() {
			var dbFilePath = "./testDbEnumeration.litedb";
			if (File.Exists(dbFilePath)) {
				File.Delete(dbFilePath);
				Thread.Sleep(TimeSpan.FromSeconds(0.05));
			}
			using (var db = new LiteDatabase($"filename={dbFilePath}; journal=false")) {
				var dbCol = db.GetCollection<DbTestPair<string, string>>("testPairs");
				dbCol.Delete(Query.All());
				var baseKeys = new[] { "A", "B", "C", "D", "E", "F", "G", "H" };
				Func<string, string> formatter = baseKey => $"testDat{baseKey}";
				foreach (var baseKey in baseKeys) {
					dbCol.Insert(new DbTestPair<string, string>(baseKey, formatter(baseKey)));
				}
				{
					var contents = dbCol.FindAll();
					var keys = contents.Select(kv => kv.Key).ToArray();
					Assert.Equal<string>(baseKeys, keys);
					var elements = contents.ToArray();
					Assert.Equal<string>(elements.Select(e => e.Value), baseKeys.Select(formatter));
					output.WriteLine(JsonConvert.SerializeObject(elements));
				}
			}
			if (File.Exists(dbFilePath)) {
				File.Delete(dbFilePath);
			}
		}

		[Fact]
		public void MetaData() {
			var dbFilePath = "./testMetaData.litedb";
			if (File.Exists(dbFilePath)) {
				File.Delete(dbFilePath);
				Thread.Sleep(TimeSpan.FromSeconds(0.05));
			}
			var db = new LiteDatabase($"filename={dbFilePath}; journal=false");
			db.DropCollection("metaData");
			using (var meta = new LiteDbMetaDataManager(db, "metaData")) {
				Assert.Empty(meta.Keys);
				var baseKeys = new[] { "A", "B", "C", "D", "E", "F", "G", "H" };
				Func<string, string> formatter = baseKey => $"testDat{baseKey}";
				foreach (var baseKey in baseKeys) {
					meta.Set(baseKey, formatter(baseKey));
				}
				{
					var keys = meta.Keys.ToArray();
					var contents = keys.Select(k => new KeyValuePair<string, string>(k, meta.Get<string>(k))).ToArray();
					Assert.Equal<string>(baseKeys, keys);
					var elements = contents.ToArray();
					Assert.Equal<string>(elements.Select(e => e.Value), baseKeys.Select(formatter));
					output.WriteLine(JsonConvert.SerializeObject(elements));
				}
				{
					var keyToDelete = meta.Keys.First();
					Assert.NotNull(meta.Get<object>(keyToDelete));
					meta.Clear(keyToDelete);
					Assert.Null(meta.Get<object>(keyToDelete));
					Assert.DoesNotContain(keyToDelete, meta.Keys);
				}
				Assert.NotEmpty(meta.Keys);
			}
			if (File.Exists(dbFilePath)) {
				File.Delete(dbFilePath);
			}
		}

		[Fact]
		public void Profiles() {
			var dbFilePath = "./testProfileData.litedb";
			if (File.Exists(dbFilePath)) {
				File.Delete(dbFilePath);
				Thread.Sleep(TimeSpan.FromSeconds(0.05));
			}
			var db = new LiteDatabase($"filename={dbFilePath}; journal=false");
			db.DropCollection("profileData");
			using (var profMgr = new LiteDbProfileManager(db, "profileData")) {
				var baseKeys = new[] { "A", "B", "C", "D", "E", "F", "G", "H" };

				var profiles = new List<MutableProfile>();
				foreach (var baseKey in baseKeys) {
					profiles.Add(new MutableProfile(Guid.NewGuid(), baseKey, new Dictionary<string, string> {
						["name"] = baseKey,
					}));
				}

				foreach (var profile in profiles) {
					profMgr.Store(profile);
				}

				foreach (var profile in profiles) {
					Assert.True(profMgr.Contains(profileId: profile.Id));
					var fetchedProfile = profMgr.Fetch(profileId: profile.Id);
					Assert.NotNull(fetchedProfile);
					Assert.Equal(profile.Name, fetchedProfile.Name);
					var prefKeysOrdered = profile.Preferences.Keys.OrderBy(x => x);
					var fetchedKeysOrdered = fetchedProfile.Preferences.Keys.OrderBy(x => x);
					Assert.Equal<string>(prefKeysOrdered, fetchedKeysOrdered);
					Assert.Equal<string>(prefKeysOrdered.Select(key => profile.Preferences[key]), fetchedKeysOrdered.Select(key => fetchedProfile.Preferences[key]));
				}

				{
					var profileCount = profMgr.Ids.Count();
					Assert.Equal(profileCount, profMgr.Entries.Count());
					{
						var profileToDelete = profiles[new Random().Next(profiles.Count)];
						profMgr.Delete(profileId: profileToDelete.Id);
						Assert.False(profMgr.Contains(profileId: profileToDelete.Id));
					}
					Assert.Equal(profileCount - 1, profMgr.Ids.Count());
				}
			}
			if (File.Exists(dbFilePath)) {
				//File.Delete(dbFilePath);
			}
		}
	}
}
