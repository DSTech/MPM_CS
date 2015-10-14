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
			var db = new LiteDatabase("filename=./testDbEnumeration.litedb; journal=false");
			var dbCol = db.GetCollection<DbTestPair<string, string>>("testPairs");
			dbCol.Delete(Query.All());
			var baseKeys = new[] { "A", "B", "C", "D", "E", "F", "G", "H" };
			Func<string, string> formatter = baseKey => $"testDat{baseKey}";
			foreach (var baseKey in baseKeys) {
				dbCol.Insert(new DbTestPair<string, string>(baseKey, formatter(baseKey)));
			}
			var contents = dbCol.FindAll();
			var keys = contents.Select(kv => kv.Key).ToArray();
			Assert.Equal<string>(baseKeys, keys);
			var elements = contents.ToArray();
			Assert.Equal<string>(elements.Select(e => e.Value), baseKeys.Select(formatter));
			output.WriteLine(JsonConvert.SerializeObject(elements));
		}

		[Fact]
		public void MetaData() {
			var db = new LiteDatabase("filename=./testMetaData.litedb; journal=false");
			db.DropCollection("metaData");
			using (var meta = new LiteDbMetaDataManager(db, "metaData")) {
				var baseKeys = new[] { "A", "B", "C", "D", "E", "F", "G", "H" };
				Func<string, string> formatter = baseKey => $"testDat{baseKey}";
				foreach (var baseKey in baseKeys) {
					meta.Set(baseKey, formatter(baseKey));
				}
				var keys = meta.Keys.ToArray();
				var contents = keys.Select(k => new MetaDataEntry(k, meta.Get<string>(k))).ToArray();
				Assert.Equal<string>(baseKeys, keys);
				var elements = contents.ToArray();
				Assert.Equal<string>(elements.Select(e => e.Value), baseKeys.Select(formatter));
				output.WriteLine(JsonConvert.SerializeObject(elements));
			}
		}

		//TODO: Implement
		//[Fact]
		//public void Profiles() {
		//}
	}
}
