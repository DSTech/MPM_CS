using System;
using MPM.Core;
using ServiceStack;

namespace MPM.Core.Dependency {
	using System.Linq;
	using Net.DTO;

	public static class NamedBuildExtensions {
		public static NamedBuild ToNamedBuild(this Package package, Build build) {
			var namedBuild = build.ConvertTo<NamedBuild>();
			namedBuild.PopulateWithNonDefaultValues(package);
			return namedBuild;
		}
		public static NamedBuild[] ToNamedBuilds(this Package package) {
			return package.Builds.Select(b => package.ToNamedBuild(b)).ToArray();
		}
	}
	public class NamedBuild : Build {
		public String Name { get; set; }
	}
}
