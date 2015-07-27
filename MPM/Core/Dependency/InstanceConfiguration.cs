using MPM.Core.Instances.Info;

namespace MPM.Core.Dependency {

	public class InstanceConfiguration {
		public static InstanceConfiguration Empty { get; } = new InstanceConfiguration { Packages = new NamedBuild[0] };
		public NamedBuild[] Packages { get; set; }
	}
}
