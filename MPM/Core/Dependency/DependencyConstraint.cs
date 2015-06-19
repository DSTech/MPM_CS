using MPM.Core.Instances.Info;

namespace MPM.Core.Dependency {
	public abstract class DependencyConstraint {
		public abstract bool Allows(NamedBuild build);
	}
}
