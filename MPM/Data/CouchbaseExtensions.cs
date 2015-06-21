using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MPM.Data {
	public static class CouchbaseJsonExtensions {
		private static object CouchifyToken(JToken token) {
			switch (token.Type) {
				case JTokenType.Object:
					return (token as JObject).ToCouch();
				case JTokenType.Array:
					return (token as JArray).Select(val => CouchifyToken(val)).ToArray();
				default:
					return token.ToString(Formatting.None);
			}
		}
		public static IDictionary<string, object> ToCouch(this JObject obj) {
			var res = new Dictionary<string, object>();
			foreach (var property in obj.Properties()) {
				res.Add(property.Name, CouchifyToken(property.Value));
			}
			return res;
		}
		public static JObject FromCouch(this IDictionary<string, object> obj) => JObject.FromObject(obj);
	}
}
