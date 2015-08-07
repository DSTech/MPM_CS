using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MPM.CLI;
using MPM.Core.Instances.Cache;
using MPM.Net.Protocols.Minecraft;
using Newtonsoft.Json;
using Nito.AsyncEx.Synchronous;
using PowerArgs;
using semver.tools;

namespace MPM {

	public class Program {
		public static void Main(string[] args) {
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
				TypeNameHandling = TypeNameHandling.Auto,
			};
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
