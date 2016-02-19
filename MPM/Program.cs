using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MPM.CLI;
using MPM.Core.Instances.Cache;
using MPM.Net.Protocols.Minecraft;
using Newtonsoft.Json;
using Nito.AsyncEx.Synchronous;
using PowerArgs;

namespace MPM {
    public static class Program {
        public static void DemoArchInstallation() {
            var mai = new MetaArchInstaller();
            var fsCache = new FileSystemCacheManager("./cache");
            var procedure = mai.EnsureCached("minecraft", Types.CompatibilitySide.Client, new MPM.Types.SemVersion("1.8.8"), fsCache, null);
            var opMap = procedure.GenerateOperations();
            foreach (var target in opMap) {
                foreach (var operation in target.Value) {
                    Console.WriteLine("{0}\n\t=> {1}", operation, target.Key);
                }
            }
            return;
        }

        public static void Main(string[] args) {
            SetupJson();
            try {
                using (var factory = new CLIFactory().GenerateResolver()) {
                    //DemoArchInstallation(); return;
                    ProcessCommandLine(factory, args);
                }
            } catch (Exception e) when (!Debugger.IsAttached) {
                Console.Error.WriteLine("\n{0}", e);
                return;
            }
        }

        public static void SetupJson() {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.Auto,
            };
        }

        public static void ProcessCommandLine(Autofac.IContainer resolver, IEnumerable<string> args) {
            ArgAction<LaunchArgs> parsed;
            try {
                parsed = Args.ParseAction<LaunchArgs>(args.ToArray());
            } catch (ArgException ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ArgUsage.GenerateUsageFromTemplate<LaunchArgs>());
                return;
            }
            if (parsed?.Args == null) {
                return;
            }
            parsed.Args.Resolver = resolver;
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
