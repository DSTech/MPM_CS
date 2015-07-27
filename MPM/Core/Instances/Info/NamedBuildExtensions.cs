using System.Linq;
using MPM.Net.DTO;
using NServiceKit.Common;

namespace MPM.Core.Instances.Info {

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
}
