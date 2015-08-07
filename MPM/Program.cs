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
		public static void DemoArchInstallation() {
			var mai = new MetaArchInstaller();
			var fsCache = new FileSystemCacheManager("./cache");
			var procedure = mai.EnsureCached("minecraft", SemanticVersion.ParseNuGet("1.8.8"), fsCache, null);
			var opMap = procedure.GenerateOperations();
			foreach (var target in opMap) {
				foreach (var operation in target.Value) {
					Console.WriteLine("{0}\n\t=> {1}", operation, target.Key);
				}
			}
			return;
		}
		public static void Main(string[] args) {
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
				TypeNameHandling = TypeNameHandling.Auto,
				Converters = new List<JsonConverter> {
					new SemanticVersionConverter(),
					new VersionSpecConverter(),
					new VersionConverter(),
				},
			};
			//DemoArchInstallation(); return;
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
