using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MPM.Data;
using LiteDB;

namespace MPM.Data {
    public class LiteDbMetaDataManager : IMetaDataManager {
        private readonly LiteCollection<MetaDataEntry> Collection;

        private JsonSerializerSettings JsonSettings = new JsonSerializerSettings {
            TypeNameHandling = TypeNameHandling.Auto,
        };

        public LiteDbMetaDataManager(LiteCollection<MetaDataEntry> metaDataCollection) {
            if ((this.Collection = metaDataCollection) == null) {
                throw new ArgumentNullException(nameof(metaDataCollection));
            }
        }

        public IEnumerable<KeyValuePair<string, object>> Pairs => Collection
            .FindAll()
            .Select(x => new KeyValuePair<string, object>(x.Key, JsonConvert.DeserializeObject(x.Value)));

        public IEnumerable<string> Keys => Collection.FindAll().Select(x => x.Key);

        public bool Contains(String key) {
            return Collection.Exists(entry => entry.Key == key);
        }

        public void Delete(String key) => Collection.Delete(kvp => kvp.Key == key);

        public void Clear() => Collection.Delete(Query.All());

        public void Set<VALUETYPE>(string key, VALUETYPE value) where VALUETYPE : class {
            if (value == null) {
                Delete(key);
                return;
            }
            Collection.Upsert(new MetaDataEntry(key, JsonConvert.SerializeObject(value, typeof(VALUETYPE), JsonSettings)));
        }

        public VALUETYPE Get<VALUETYPE>(string key) where VALUETYPE : class {
            var stored = Collection.FindOne(kvp => kvp.Key == key)?.Value;
            if (stored == null) {
                return null;
            }
            return JsonConvert.DeserializeObject<VALUETYPE>(stored, JsonSettings);
        }

        public VALUETYPE Get<VALUETYPE>(string key, VALUETYPE defaultValue) where VALUETYPE : class {
            var stored = Collection.FindOne(kvp => kvp.Key == key)?.Value;
            if (stored == null) {
                return defaultValue;
            }
            return JsonConvert.DeserializeObject<VALUETYPE>(stored, JsonSettings);
        }

        public class MetaDataEntry {
            public MetaDataEntry() {
            }

            public MetaDataEntry(string key, string value) {
                this.Key = key;
                this.Value = value;
            }

            [BsonId]
            public string Key { get; set; }

            [BsonField]
            public string Value { get; set; }

            public static implicit operator KeyValuePair<string, string>(MetaDataEntry entry) => new KeyValuePair<string, string>(entry.Key, entry.Value);
            public static implicit operator MetaDataEntry(KeyValuePair<string, string> pair) => new MetaDataEntry(pair.Key, pair.Value);
        }
    }
}
