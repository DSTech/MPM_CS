using System;
using System.Collections.Generic;
using System.Linq;
using MPM.Extensions;
using MPM.Types;

namespace MPM.Net.Protocols.Minecraft.Types {

    public class LibraryRuleSpec {
        public LibraryRuleSpec() {
        }
        public LibraryRuleSpec(LibraryRuleAction action, IEnumerable<LibraryRuleFilter> filters) {
            this.Action = action;
            this.Filters = filters.Denull().ToList();
        }
        public LibraryRuleAction Action { get; set; }
        public List<LibraryRuleFilter> Filters { get; set; }
        public bool IsDefault => Filters.Count == 0;
        public bool Applies(PlatformID platform, bool x64 = true) {
            return Filters.All(filter => filter.Applies(platform, x64));
        }
    }
}
