using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using MPM.Extensions;
using Newtonsoft.Json;

namespace MPM.Core.Profiles {
	public static class MutableProfileExtensions {
		public static MutableProfile ToMutableProfile(this IProfile profile) {
			return new MutableProfile {
				Name = profile.Name,
				Preferences = profile.Preferences.ToDictionary(x => x.Key, x => x.Value),
			};
		}
	}
	public class MutableProfile : IProfile {
		public MutableProfile() {
		}

		public MutableProfile(String name, IReadOnlyDictionary<string, string> preferences = null) {
			this.Name = name;
			this.Preferences = preferences?.ToDictionary(pair => pair.Key, pair => pair.Value);
		}

		[BsonField, BsonIndex]
		public string Name { get; set; }

		[BsonField]
		public Dictionary<string, string> Preferences { get; set; } = new Dictionary<string, string>();

		[BsonIgnore]
		IReadOnlyDictionary<string, string> IProfile.Preferences => Preferences.AsReadOnly();
	}
}
