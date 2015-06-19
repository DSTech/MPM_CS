using System;
using MPM.Core.Instances.Installation.Scripts;
using semver.tools;

namespace MPM.Core.Instances.Info {
	public class ScriptFileDeclaration {
		public String Type { get; set; }
		public String Description { get; set; }
		public String Source { get; set; }
		public String Hash { get; set; }
		public String Target { get; set; }
		public String[] Targets { get; set; }
		public IFileDeclaration Parse(String packageName, SemanticVersion packageVersion) {
			//Determine what type of file declaration to create depending upon class members
			if (Source == null && Type != null) {
				var decl = new SourcelessFileDeclaration() {
					Description = Description,
					PackageName = packageName,
					PackageVersion = packageVersion,
					Targets = (Target != null ? new[] { Target } : Targets ?? new String[0]),
				};
				switch (Type) {
					case "configuration":
						decl.Type = SourcelessType.Configuration;
						return decl;
					case "cache":
						decl.Type = SourcelessType.Cache;
						return decl;
					default:
						throw new NotSupportedException($"File declaration type {Type} not supported");
				}
			}
			//TODO: Add support for other declaration types
			{
				var e = new NotSupportedException($"File declaration not supported");
				e.Data.Add(nameof(ScriptFileDeclaration), this);
				throw e;
			}
		}
	}
}
