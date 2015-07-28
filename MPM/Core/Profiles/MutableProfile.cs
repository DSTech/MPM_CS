using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.Profiles {

	public class MutableProfile : IProfile {

		public MutableProfile() {
		}

		public MutableProfile(Guid id, String name, IReadOnlyDictionary<string, string> preferences = null) {
			this.Id = id;
			this.Name = name;
			this.Preferences = preferences?.ToDictionary(pair => pair.Key, pair => pair.Value);
		}

		public Guid Id { get; set; }

		public string Name { get; set; }

		public IDictionary<string, string> Preferences = new Dictionary<string, string>();

		IReadOnlyDictionary<string, string> IProfile.Preferences => new ReadOnlyDictionary<string, string>(Preferences);
	}
}