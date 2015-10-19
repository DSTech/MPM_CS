using System.Collections.Generic;
using LiteDB;

namespace MPM.Data {
	public class MetaDataEntry {
		public MetaDataEntry() {
		}
		public MetaDataEntry(string key, string value) {
			this.Key = key;
			this.Value = value;
		}
		[LiteDB.BsonId]
		public string Key { get; set; }
		[LiteDB.BsonField]
		public string Value { get; set; }
		public static implicit operator KeyValuePair<string, string>(MetaDataEntry entry) => new KeyValuePair<string, string>(entry.Key, entry.Value);
		public static implicit operator MetaDataEntry(KeyValuePair<string, string> pair) => new MetaDataEntry(pair.Key, pair.Value);
	}
}
