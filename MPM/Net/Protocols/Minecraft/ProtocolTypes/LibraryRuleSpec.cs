using System;
using System.Collections.Generic;
using System.Linq;
using MPM.Extensions;
using MPM.Types;
using Newtonsoft.Json;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    public class LibraryRuleSpec {
        public LibraryRuleSpec() {
        }

        public LibraryRuleSpec(LibraryRuleAction action, LibraryRuleFilterOS osFilter = null) {
            this.Action = action;
            this.OsFilter = osFilter;
        }

        [JsonProperty("action", NullValueHandling = NullValueHandling.Ignore)]
        public LibraryRuleAction Action { get; set; }

        [JsonIgnore]
        public List<LibraryRuleFilter> Filters => new LibraryRuleFilter[] {
            this.OsFilter,
        }.Where(x => x != null).ToList();

        [JsonIgnore]
        public bool IsDefault => Filters.Count == 0;

        [JsonProperty("os", NullValueHandling = NullValueHandling.Ignore)]
        public LibraryRuleFilterOS OsFilter { get; set; }

        public bool Applies(PlatformID platform, bool x64 = true) {
            return Filters.All(filter => filter.Applies(platform, x64));
        }
    }
}
