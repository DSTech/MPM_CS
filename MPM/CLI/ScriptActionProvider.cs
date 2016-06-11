using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using Autofac;
using ICSharpCode.SharpZipLib.Zip;
using MPM.Core.Instances.Installation.Scripts;
using MPM.Types;
using Newtonsoft.Json;
using MPM.Util;
using PowerArgs;
using IronPython.Hosting;

namespace MPM.CLI {
    public partial class RootArgs {
        [ArgActionMethod]
        [ArgShortcut(ArgShortcutPolicy.ShortcutsOnly), ArgShortcut("script"), ArgShortcut("python")]
        public void ExecuteScript(ScriptArgs args) {
            var scriptActionProvider = new ScriptActionProvider();
            scriptActionProvider.Provide(Resolver, args);
        }
    }

    public class ScriptActionProvider : IActionProvider<ScriptArgs> {
        public void Provide(IContainer factory, ScriptArgs args) {
            var appDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine($"{args.ScriptName}({args.SubArgs.Join(",")}) =>");

            switch (args.ScriptName.ToLowerInvariant()) {
                //case "": break;//One case for each hardcoded "script", anything too small or niche-case to include in the full CLI
                default: {
                        var python = Python.CreateEngine();
                        var pyScope = python.CreateScope();
                        {
                            foreach (var file in appDir.CreateSubdirectory("Scripts").EnumerateFiles("*.py")) {
                                python.ExecuteFile(file.FullName, pyScope);
                            }
                            using (new ConsoleIndenter()) {
                                Console.WriteLine(python.ExecuteAndWrap($"{args.ScriptName}({args.SubArgs.Select(a => $"\"{a}\"").Join(",")})", pyScope).Unwrap());
                            }
                        }
                    }
                    break;
            }
        }
    }
}
