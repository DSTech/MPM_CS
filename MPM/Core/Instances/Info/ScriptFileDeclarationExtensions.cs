using System;
using MPM.Core.Instances.Installation.Scripts;
using semver.tools;

namespace MPM.Core.Instances.Info {
	public static class ScriptFileDeclarationExtensions {
		public static IFileDeclaration Parse(this ScriptFileDeclaration declaration, String packageName, SemanticVersion packageVersion) {
			//Determine what type of file declaration to create depending upon class members
			if (declaration.Source == null && declaration.Type != null) {
				var decl = new SourcelessFileDeclaration() {
					Description = declaration.Description,
					PackageName = packageName,
					PackageVersion = packageVersion,
					Targets = (declaration.Target != null ? new[] { declaration.Target } : declaration.Targets ?? new String[0]),
				};
				switch (declaration.Type) {
					case "configuration":
						decl.Type = SourcelessType.Configuration;
						return decl;
					case "cache":
						decl.Type = SourcelessType.Cache;
						return decl;
					default:
						throw new NotSupportedException($"File declaration type {declaration.Type} not supported");
				}
			}
			//TODO: Add support for other declaration types
			{
				var e = new NotSupportedException($"File declaration not supported");
				e.Data.Add(nameof(ScriptFileDeclaration), declaration);
				throw e;
			}
		}
	}
}
