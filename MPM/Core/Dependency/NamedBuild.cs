using System;
using MPM.Core;

namespace MPM.Core.Dependency {
	using System.Linq;
	using Net.DTO;
	using NServiceKit.Common;

	public static class NamedBuildExtensions {
		public static NamedBuild ToNamedBuild(this Package package, Build build) {
			var namedBuild = new NamedBuild().PopulateWithNonDefaultValues(build);
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
