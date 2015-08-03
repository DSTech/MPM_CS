using System;
using System.Collections.Generic;
using MPM.CLI;
using MPM.Net.Protocols.Minecraft;
using Newtonsoft.Json;
using Nito.AsyncEx.Synchronous;
using PowerArgs;

namespace MPM {

	public class Program {

		public static void Main(string[] args) {
			var mdc = new MinecraftDownloadClient();
			var latestSnapshot = mdc.FetchLatestSnapshot().WaitAndUnwrapException();
			Console.WriteLine(latestSnapshot);
			ArgAction<LaunchArgs> parsed;
			try {
				parsed = Args.ParseAction<LaunchArgs>(args);
			} catch (ArgException ex) {
				Console.WriteLine(ex.Message);
				Console.WriteLine(ArgUsage.GenerateUsageFromTemplate<LaunchArgs>());
				return;
			}
			if (parsed?.Args == null) {
				return;
			}
			try {
				parsed.Invoke();
			} catch (ArgException ex) {
				Console.WriteLine(ex.Message);
				Console.WriteLine(ArgUsage.GenerateUsageFromTemplate<LaunchArgs>());
				return;
			}
		}
	}
}
