using MPM.Core.Instances.Info;
using MPM.Types;

namespace MPM.Core.Dependency {
    public abstract class DependencyConstraint {
        public abstract bool Allows(Build build);
    }
}
