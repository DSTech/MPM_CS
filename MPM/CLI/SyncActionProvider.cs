using System;
using Autofac;
using PowerArgs;

namespace MPM.CLI {
    public partial class RootArgs {
        [ArgActionMethod]
        [ArgShortcut("s"), ArgShortcut("--sync")]
        public void Sync(SyncArgs args) {
            var syncActionProvider = new SyncActionProvider();
            syncActionProvider.Provide(Resolver, args);
        }
    }

    public class SyncActionProvider : IActionProvider<SyncArgs> {
        public void Provide(IContainer resolver, SyncArgs args) {
            throw new NotImplementedException();
        }
    }
}
