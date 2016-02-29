using Autofac;

namespace MPM.CLI {
    public interface IActionProvider<in TArg> where TArg : ICommandLineArgs {
        void Provide(IContainer resolver, TArg args);
    }
}
