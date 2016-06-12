using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using MPM.Core.Instances.Installation.Scripts;
using MPM.Types;
using Newtonsoft.Json;
using MPM.Util;
using PowerArgs;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using Alphaleonis.Win32.Filesystem;
using MPM.Util.Json;
using Newtonsoft.Json.Linq;
using PowerArgs.Cli;
using IContainer = Autofac.IContainer;

namespace MPM.CLI {
    // ReSharper disable once ClassNeverInstantiated.Global
    public partial class RootArgs {
        [ArgActionMethod]
        public void PrepForge(PrepForgeArgs args) {
            var prepForgeActionProvider = new PrepForgeActionProvider();
            prepForgeActionProvider.Provide(Resolver, args);
        }
    }

    public class PrepForgeActionProvider : IActionProvider<PrepForgeArgs> {
        public void Provide(IContainer factory, PrepForgeArgs args) {
            string forgeUrl;
            string chosenForgeVersion;
            using (ConsoleColorZone.Info) {
                Console.Write("Resolving Forge version to URL... ");
                try {
                    var choice = ResolveForgeVersion(minecraftVersion: new SemVersion(args.MinecraftVersion), forgeVersion: args.ForgeVersion);
                    forgeUrl = choice.Url;
                    chosenForgeVersion = choice.Version;
                } catch (ApplicationException e) {
                    using (ConsoleColorZone.Error) {
                        Console.WriteLine("Failed!");
                        Console.WriteLine($"\t{e.Message}");
                    }
                    return;
                }
                using (ConsoleColorZone.Success) {
                    Console.WriteLine("Done.");
                }
                using (ConsoleColorZone.Info) {
                    Console.WriteLine($"\t{forgeUrl}");
                }
            }
            Console.WriteLine($"Preparing Forge at path:\n\t{args.PackageDirectory}");
            var packageDir = args.PackageDirectory;
            if (!packageDir.Exists) { packageDir.Create(); }
            Environment.CurrentDirectory = packageDir.FullName;

            // 12.17.0.1954_b => 17.0.1954-b
            var forgeVersionAsSemVer = new SemVersion(chosenForgeVersion.Substring(chosenForgeVersion.IndexOf('.') + 1).Replace('_', '-'), true);
            var buildInfo = new Build() {
                PackageName = "Forge",
                Arch = new SemVersion(args.MinecraftVersion),
                Authors = new List<Author>(new Author[] { new Author("Forge Authors", "") }),
                Conflicts = new List<Conflict>(new Conflict[] {
                    new Conflict(
                        new ConflictCondition(packageName: "Bukkit"),//TODO: Verify that this is how conflicts are defined
                        new ConflictResolution(
                            new DependencyConflictResolution(
                                new ForcedDependencySet(),
                                new DeclinedDependencySet(packageNames: new string[] { "Bukkit" })
                            ),
                            new InstallationConflictResolution()
                        )
                    )
                }),
                Side = CompatibilitySide.Universal,
                GivenVersion = args.ForgeVersion,
                Version = forgeVersionAsSemVer,
                Dependencies = new BuildDependencySet(),
                Interfaces = new List<InterfaceProvision>(new[] {
                    new InterfaceProvision("Forge", forgeVersionAsSemVer)
                }),
            };



            File.WriteAllText(packageDir.SubFile("package.json").FullName, JsonConvert.SerializeObject(buildInfo, Formatting.Indented));
        }

        protected class ForgeInstallProfile {
            [JsonProperty(PropertyName = "versionInfo")]
            public ForgeInstallVersionInfo VersionInfo { get; set; }
        }

        protected class ForgeInstallVersionInfo {
            [JsonProperty(PropertyName = "libraries")]
            public List<ForgeInstallLibraryEntry> Libraries { get; set; }
        }

        protected class ForgeInstallLibraryEntry {
            [JsonProperty(PropertyName = "name")]// eg "org.scala-lang:scala-compiler:2.11.1"
            public string Name { get; set; }

            [JsonProperty(PropertyName = "url")]// eg "http://files.minecraftforge.net/maven/"
            public string Url { get; set; } = null;

            [JsonProperty(PropertyName = "serverreq")]
            public bool ServerReq { get; set; } = false;

            [JsonProperty(PropertyName = "clientreq")]
            public bool ClientReq { get; set; } = false;

            [JsonProperty(PropertyName = "checksums")]// eg ["56ea2e6c025e0821f28d73ca271218b8dd04926a", "1444992390544ba3780867a13ff696a89d7d1639"]
            public List<string> Checksums { get; set; }
        }

        protected class ForgeVersionEntry {
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "time")]
            public string Time { get; set; }
        }

        protected class ForgeVersionCollection {
            [JsonProperty(PropertyName = "latest")]
            public Dictionary<string, string> Latest { get; set; }

            [JsonProperty(PropertyName = "recommended")]
            public Dictionary<string, string> Recommended { get; set; }

            [JsonProperty(PropertyName = "versions")]
            public Dictionary<string, List<ForgeVersionEntry>> Versions { get; set; }
        }

        private struct ForgeVersionChoice {
            public string Url { get; set; }
            public string Version { get; set; }
        }

        private ForgeVersionChoice ResolveForgeVersion(SemVersion minecraftVersion, string forgeVersion) {
            ForgeVersionCollection forgeVersions;
            using (var wc = new WebClient()) {
                var versionsJson = wc.DownloadString("http://thej89.dessix.net/mmdb/forge/versions.json");
                forgeVersions = JsonConvert.DeserializeObject<ForgeVersionCollection>(versionsJson, new VersionConverter(loose: true), new VersionRangeConverter(loose: true));
            }
            var lowerForgeVersion = forgeVersion.ToLowerInvariant();
            if (lowerForgeVersion == "latest") {
                var latest = forgeVersions.Latest;
                forgeVersion = latest
                    .Where(f => minecraftVersion.ToString().StartsWith(f.Key.ToLowerInvariant()))
                    .OrderByDescending(v => v.Key.Length)
                    .ThenByDescending(pair => pair.Key)
                    .Select(pair => pair.Value)
                    .FirstOrDefault();
            } else if (forgeVersion.ToLowerInvariant() == "recommended") {
                var recommended = forgeVersions.Recommended;
                forgeVersion = recommended
                    .Where(f => minecraftVersion.ToString().StartsWith(f.Key.ToLowerInvariant()))
                    .OrderByDescending(v => v.Key.Length)
                    .ThenByDescending(pair => pair.Key)
                    .Select(pair => pair.Value)
                    .FirstOrDefault();
            }
            var versions = forgeVersions.Versions.Where(v => v.Key == minecraftVersion.ToString()).Select(v => v.Value).FirstOrDefault()?.OrderByDescending(v => v.Name);
            forgeVersion = versions.FirstOrDefault(v => v.Name == forgeVersion)?.Name;

            if (forgeVersion == null || versions == null || versions.All(v => v.Name != forgeVersion)) {
                throw new ApplicationException("Provided Forge version not found.");
            }

            return new ForgeVersionChoice {
                Url = $"http://thej89.dessix.net/mmdb/forge/{minecraftVersion}/{forgeVersion}/install_profile.json",
                Version = forgeVersion,
            };
        }
    }
}
