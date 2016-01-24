using System;
using System.Collections.Generic;
using System.Linq;
using MPM.Types;

namespace MPM.Net.Protocols.Minecraft.Types {

	public class LibrarySpec {
		public LibrarySpec() {
		}

		public LibrarySpec(string package, string name, string version, LibraryNativesSpec nativesSpec, LibraryExtractSpec extractSpec, IEnumerable<LibraryRuleSpec> rules) {
			this.Package = package;
			this.Name = name;
			this.Version = version;
			this.Natives = nativesSpec;
			this.Extract = extractSpec;
			this.Rules = rules.ToList();
		}

		/// <summary>
		/// Package of the library being specified, eg: "com.google.code.gson"
		/// </summary>
		public string Package { get; set; }

		/// <summary>
		/// Name of the library being specified, eg: "gson"
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Version of the library being specified, eg: "2.2.4"
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		/// A description of native libraries to be used to supplement a particular library
		/// </summary>
		public LibraryNativesSpec Natives { get; set; }

		public string ApplyNatives(PlatformID platform, bool x64 = true) => Natives?.AppliedTo(platform, x64);

		/// <summary>
		/// A description of how a package should be extracted, with optional exclusions
		/// </summary>
		public LibraryExtractSpec Extract { get; set; }

		/// <summary>
		/// A series of rules for whether or not a library is allowed on an operating system. Later entries take precedence over previous ones
		/// </summary>
		public List<LibraryRuleSpec> Rules { get; set; }

		public bool Applies(PlatformID platform) {
			if (Rules.Count == 0) {
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
