using System;
using System.Collections.Generic;
using System.Linq;
using MPM.Types;
using Newtonsoft.Json;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    public class LibrarySpec {
        public LibrarySpec() {
        }

        public LibrarySpec(string package, string name, string version, LibraryNativesSpec nativesSpec, LibraryExtractSpec extractSpec, IEnumerable<LibraryRuleSpec> rules) {
            this._identity = new LibraryIdentity {
                Package = package,
                Name = name,
                Version = version,
            };
            this.Natives = nativesSpec;
            this.Extract = extractSpec;
            this.Rules = rules.ToList();
        }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore, Order = 2)]
        private LibraryIdentity _identity;

        #region PseudoProperties

        /// <summary>
        ///     Package of the library being specified, eg: "com.google.code.gson"
        /// </summary>
        [JsonIgnore]
        public string Package {
            get { return _identity.Package; }
            set { _identity.Package = value; }
        }

        /// <summary>
        ///     Name of the library being specified, eg: "gson"
        /// </summary>
        [JsonIgnore]
        public string Name {
            get { return _identity.Name; }
            set { _identity.Name = value; }
        }

        /// <summary>
        ///     Version of the library being specified, eg: "2.2.4"
        /// </summary>
        [JsonIgnore]
        public string Version {
            get { return _identity.Version; }
            set { _identity.Version = value; }
        }

        #endregion

        /// <summary>
        ///     A description of native libraries to be used to supplement a particular library
        /// </summary>
        [JsonProperty("natives", NullValueHandling = NullValueHandling.Ignore, Order = 3)]
        public LibraryNativesSpec Natives { get; set; }

        /// <summary>
        ///     A description of how a package should be extracted, with optional exclusions
        /// </summary>
        [JsonProperty("extract", NullValueHandling = NullValueHandling.Ignore, Order = 1)]
        public LibraryExtractSpec Extract { get; set; }

        /// <summary>
        ///     A series of rules for whether or not a library is allowed on an operating system. Later entries take precedence
        ///     over previous ones
        /// </summary>
        [JsonProperty("rules", NullValueHandling = NullValueHandling.Ignore, Order = 4)]
        public List<LibraryRuleSpec> Rules { get; set; }

        [JsonProperty("downloads", NullValueHandling = NullValueHandling.Ignore, Order = 5)]
        public DownloadsSpec Downloads { get; set; }

        public string ApplyNatives(PlatformID platform, bool x64 = true) => Natives?.AppliedTo(platform, x64);

        public bool Applies(PlatformID platform) {
            if ((Rules?.Count ?? 0) == 0) {
                //No rules means no restrictions
                return true;
            }
            var defaultRule = Rules.FirstOrDefault();
            var otherRules = Rules.Where(rule => !rule.IsDefault).ToArray();
            foreach (var rule in otherRules.Where(rule => rule.Applies(platform))) {
                switch (rule.Action) {
                    case LibraryRuleAction.Allow:
                        return true;
                    case LibraryRuleAction.Disallow:
                        return false;
                    default:
                        throw new NotSupportedException();
                }
            }
            if (defaultRule != null) {
                //None of the rules applied, but default existed
                //so return the default rule's action
                switch (defaultRule.Action) {
                    case LibraryRuleAction.Allow:
                        return true;
                    case LibraryRuleAction.Disallow:
                        return false;
                    default:
                        throw new NotSupportedException();
                }
            }
            return false;//No default rule existed, and no other rules allowed the library
        }
    }
}
