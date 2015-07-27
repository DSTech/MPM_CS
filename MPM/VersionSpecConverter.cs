using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using semver.tools;

namespace MPM {

	public class VersionSpecConverter : JsonConverter {

		public override bool CanConvert(Type objectType) {
			return (objectType == typeof(VersionSpec));
		}

		public override bool CanRead {
			get {
				return true;
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			var tok = JToken.Load(reader);
			var tokStr = tok.ToString();
			return VersionSpec.ParseNuGet(tokStr);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			var val = (VersionSpec)value;
			writer.WriteValue(val.ToStringNuGet());
		}
	}
}
