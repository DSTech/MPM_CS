using MPM.ActionProviders;
using MPM.CLI;

namespace MPM.Extensions {
    public static class ActionProviderArgX {
        public static MinecraftLauncher ToConfiguredLauncher(this LaunchMinecraftArgs self) {
            return new MinecraftLauncher {
                UserName = self.UserName,
            };
        }
    }
}
