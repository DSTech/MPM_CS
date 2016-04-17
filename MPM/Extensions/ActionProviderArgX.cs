using MPM.ActionProviders;

namespace MPM.CLI {
    public static class ActionProviderArgX {
        public static MinecraftLauncher ToConfiguredLauncher(this LaunchMinecraftArgs self) {
            return new MinecraftLauncher {
                UserName = self.UserName,
            };
        }
    }
}
