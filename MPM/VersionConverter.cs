using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using semver.tools;

namespace MPM {

	public class VersionConverter : JsonConverter {

		public override bool CanConvert(Type objectType) => (objectType == typeof(Net.DTO.Version));

		public override bool CanRead => true;

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			var tok = JToken.Load(reader);
			var tokStr = tok.ToString();
			return Net.DTO.Version.Parse(tokStr);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			var val = (Net.DTO.Version)value;
			writer.WriteValue(val.ToString());
		}
	}
}
