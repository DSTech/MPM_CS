using System.Collections.Generic;
using System.Linq;
using MPM.Extensions;
using MPM.Types;

namespace MPM.Net.Protocols.Minecraft.Types {

	public class LibraryRuleSpec {
		public LibraryRuleSpec(LibraryRuleAction action, IEnumerable<LibraryRuleFilter> filters) {
			this.Action = action;
			this.Filters = filters.Denull().ToArray();
		}
		public LibraryRuleAction Action { get; }
		public IReadOnlyList<LibraryRuleFilter> Filters { get; }
		public bool IsDefault => Filters.Count == 0;
		public bool Applies(CompatibilityPlatform platform) {
			return Filters.All(filter => filter.Applies(platform));
		}
	}
}
