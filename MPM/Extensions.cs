using System;
using System.Collections;
using MPM.CLI;
using NServiceKit.Text;

namespace MPM {
	public static class ActionProviderArgExtensions {
		public static MinecraftLauncher ToConfiguredLauncher(this LaunchMinecraftArgs self) {
			return new MinecraftLauncher {
				UserName = self.UserName,
			};
		}
	}
}
