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
            var buildInfo = CreateForgeBuildInfo(args, forgeVersionAsSemVer);

            var forgeInstallProfile = FetchInstallProfile(forgeUrl);

            AddForgeLibrariesToBuild(args.PackageDirectory, args.MinecraftVersion, chosenForgeVersion, forgeInstallProfile, args.Side, ref buildInfo);

            File.WriteAllText(packageDir.SubFile("package.json").FullName, JsonConvert.SerializeObject(buildInfo, Formatting.Indented));
        }

        private static Build CreateForgeBuildInfo(PrepForgeArgs args, SemVersion forgeVersionAsSemVer) {
            var buildInfo = new Build() {
                PackageName = "minecraftforge",
                Arch = new SemVersion(args.MinecraftVersion),
                Authors = new List<Author>(new Author[] { new Author("Forge Authors", "") }),
                Conflicts = new List<Conflict>(new Conflict[] {
                    new Conflict(//TODO: Make a fluent interface for declaring conflicts
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
                Side = args.Side,
                GivenVersion = args.ForgeVersion,
                Version = forgeVersionAsSemVer,
                Dependencies = new BuildDependencySet(),
                Interfaces = new List<InterfaceProvision>(new[] {
                    new InterfaceProvision("Forge", forgeVersionAsSemVer)
                }),
            };
            return buildInfo;
        }

        #region ForgeListings

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

        #endregion ForgeListings

        #region ForgeInstallProfiles

        public class ForgeInstallProfile {
            [JsonProperty(PropertyName = "versionInfo")]
            public ForgeInstallVersionInfo VersionInfo { get; set; }
        }

        public class ForgeInstallVersionInfo {
            [JsonProperty(PropertyName = "libraries")]
            public List<ForgeInstallLibraryEntry> Libraries { get; set; }
        }

        public class ForgeInstallLibraryEntry {
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

        public ForgeInstallProfile FetchInstallProfile(string forgeInstallProfileUrl) {
            ForgeInstallProfile installProfile;
            using (var wc = new WebClient()) {
                var versionsJson = wc.DownloadString(forgeInstallProfileUrl);
                installProfile = JsonConvert.DeserializeObject<ForgeInstallProfile>(versionsJson, new VersionConverter(loose: true), new VersionRangeConverter(loose: true));
            }
            return installProfile;
        }

        private void AddForgeLibrariesToBuild(DirectoryInfo buildDirectory, string minecraftVersion, string forgeVersion, ForgeInstallProfile forgeInstallProfile, CompatibilitySide side, ref Build buildInfo) {
            var libs = forgeInstallProfile.VersionInfo.Libraries.Where(lib => lib.Url != null);
            var libsToInstall = libs.Where(lib => lib.ServerReq || lib.ClientReq).ToList();
            var installation = new List<IFileDeclaration>();

            // Add Forge to 
            libsToInstall.Insert(0, new ForgeInstallLibraryEntry {
                Name = libs.FirstOrDefault(l => l.Name.StartsWith("net.minecraftforge:forge")).Name,
                Url = GetForgeUniversalUrl(minecraftVersion, forgeVersion),
                ClientReq = true,
                ServerReq = true,
            });

            using (var wc = new WebClient()) {
                foreach (var lib in libsToInstall) {
                    switch (side) {
                        case CompatibilitySide.Universal:
                            if (!lib.ClientReq && !lib.ServerReq) {
                                continue;
                            }
                            break;
                        case CompatibilitySide.Client:
                            if (!lib.ClientReq) {
                                continue;
                            }
                            break;
                        case CompatibilitySide.Server:
                            if (!lib.ServerReq) {
                                continue;
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(side), side, null);
                    }

                    var libPath = AssembleLibraryPath(lib.Name);
                    string libUrl;
                    if (!lib.Name.StartsWith("net.minecraftforge:forge")) {
                        libUrl = AssembleLibraryUrl(lib.Url, lib.Name);
                    } else {
                        libUrl = lib.Url;
                    }

                    var localPath = $"libraries/{libPath}";
                    var targetPath = localPath;
                    var localFile = buildDirectory.SubFile(localPath);
                    localFile.Directory.Create();

                    if (!localFile.Exists) {
                        try {
                            var packed = wc.DownloadData($"{libUrl}.pack.xz");
                            File.WriteAllBytes(localFile.FullName, Unpack(packed));
                        } catch (WebException) {
                            wc.DownloadFile(libUrl, localFile.FullName);
                        }
                    }

                    var fileDec = new SourcedFileDeclaration {
                        Description = lib.Name,
                        Source = localPath,
                        Targets = new[] { targetPath },
                    };

                    installation.Add(fileDec);
                    if (lib.Name.StartsWith("net.minecraftforge:forge") && (side == CompatibilitySide.Server || side == CompatibilitySide.Universal)) {
                        fileDec.Targets = fileDec.Targets.Concat(new[] { "minecraftforge.jar" }).ToArray();
                    }
                }
            }

            buildInfo.Installation = installation;
        }

        private static string AssembleLibraryPath(string libraryName) {
            var nameParts = libraryName.Split(new[] { ':' });
            return $"{nameParts[0].Replace('.', '/')}/{nameParts[1]}/{nameParts[2]}/{nameParts[1]}-{nameParts[2]}.jar";
        }

        private static string AssembleLibraryUrl(string baseUrl, string libraryName) {
            var libraryPath = AssembleLibraryPath(libraryName);
            return $"{baseUrl}{libraryPath}";
        }

        private static string GetForgeUniversalUrl(string minecraftVersion, string forgeVersion) {
            JObject forgeListing;
            using (var wc = new WebClient()) {
                var forgeListingJson = wc.DownloadString($"http://thej89.dessix.net/mmdb/forge/{minecraftVersion}/{forgeVersion}/{forgeVersion}.json");
                forgeListing = JObject.Parse(forgeListingJson);
            }
            var downloads = forgeListing["downloads"].AsJEnumerable();
            var url = downloads.FirstOrDefault(i => i["type"].ToString() == "universal")?["url"]?.ToString();
            return url;
        }

        private static byte[] Unpack(byte[] packed) {
            var unpacked = ManagedXZ.XZUtils.DecompressBytes(packed, 0, packed.Length);
            if (unpacked.Length < 8) {
                throw new FormatException("FileSize is too small.");
            }
            var checksumLength = BitConverter.ToUInt32(unpacked, unpacked.Length - 8);
            if (checksumLength > unpacked.Length - 8) {
                throw new FormatException("Invalid checksum file size.");
            }
            var tempPackedFile = new FileInfo(Path.GetTempFileName());
            var tempUnpackedFile = new DirectoryInfo(Path.GetTempPath()).SubFile($"{tempPackedFile.Name}_unpacked");
            using (var file = File.OpenWrite(tempPackedFile.FullName)) {
                file.Write(unpacked, 0, Convert.ToInt32(unpacked.Length - (8 + checksumLength)));
            }
            JavaLauncher.Unpack200(tempPackedFile.FullName, tempUnpackedFile.FullName);

            var output = File.ReadAllBytes(tempUnpackedFile.FullName);
            if (tempPackedFile.Exists) { tempPackedFile.Delete(); }
            if (tempUnpackedFile.Exists) { tempUnpackedFile.Delete(); }
            return output;
        }

        #endregion ForgeInstallProfiles
    }
}
