using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Cache;
using MPM.Core.Instances.Installation;
using MPM.Core.Protocols;
using MPM.Extensions;
using MPM.Net.Protocols.Minecraft.ProtocolTypes;
using MPM.Util;
using Nito.AsyncEx.Synchronous;

namespace MPM.Net.Protocols.Minecraft {
    public class MinecraftArchInstaller {
        public IArchInstallationProcedure EnsureCached(MPM.Types.SemVersion archVersion, ICacheManager cacheManager, IProtocolResolver protocolResolver) {
            var mdc = new MinecraftDownloadClient();
            MinecraftVersion versionDetails;
            {
                Console.WriteLine("Fetching minecraft details");
                var elapsedTime = TimerUtil.Time(out versionDetails, () => {
                    versionDetails = mdc.FetchVersion(archVersion.ToString());
                });
                Console.WriteLine("Fetched details for {0} in {1}ms", versionDetails.Id, elapsedTime.TotalMilliseconds.ToString());
            }
            var libsToInstall = versionDetails.Libraries.Where(lib => lib.AppliesToPlatform(Environment.OSVersion.Platform));

            //TODO: Download client and server jars

            //versionDetails.downloads

            //TODO: Download assets
            var installingPlatform = Environment.OSVersion.Platform;
            var installingBitness64 = Environment.Is64BitOperatingSystem;

            var operations = new List<ArchInstallationOperation>();
            foreach (var lib in libsToInstall) {
                if (!lib.AppliesToPlatform(installingPlatform)) {
                    continue;
                }

                var _nativeDetails = lib.ApplyNatives(installingPlatform, installingBitness64);
                var nativeArtifact = _nativeDetails.Artifact;
                string nativeTag = null;
                var nativeClause = "";
                if (nativeArtifact != null) {
                    nativeTag = _nativeDetails.Tag;
                    nativeClause = $"_{installingPlatform}_{(installingBitness64 ? "64" : "32")}";
                }

                Debug.Assert(nativeArtifact != null || lib.Downloads.Artifact != null, "At least one artifact should be available if a library applies for this platform");
                var cacheEntryName = $"{lib.Package}_{lib.Name}_{lib.Version}{nativeClause}";
                var cacheOp = GenerateOp(
                    archVersion,
                    lib,
                    nativeArtifact ?? lib.Downloads.Artifact,
                    nativeTag,
                    cacheEntryName,
                    cacheManager
                    );
                operations.Add(cacheOp);
                if (cacheManager.Contains(cacheEntryName)) {
                    Console.WriteLine($"File already cached as {cacheEntryName}.");
                    continue;
                }
                Console.WriteLine($"Fetching {lib.Name}...");
                var libStream = mdc.FetchArtifact(nativeArtifact ?? lib.Downloads.Artifact).WaitAndUnwrapException();
                Console.WriteLine($"Writing to cache {cacheEntryName}...");
                using (libStream) {
                    cacheManager.Store(cacheEntryName, libStream.ReadToEnd());
                }
            }
            return new ArchInstallationProcedure(operations.ToArray());
        }

        private ArchInstallationOperation GenerateOp(
            MPM.Types.SemVersion archVersion,
            LibrarySpec lib,
            ArtifactSpec artifact,
            string nativeTagOrNull,
            string cacheEntryName,
            ICacheManager cacheManager
            ) {
            if (lib.Extract != null) {
                //generate extract def
                string extractDestination = nativeTagOrNull != null ? "bin/natives/" : "bin/";
                return new ExtractArchInstallationOperation(
                    packageName: lib.Name,
                    packageVersion: archVersion,
                    cacheManager: cacheManager,
                    cachedName: cacheEntryName,
                    sourcePath: "",
                    targetPath: extractDestination,
                    ignorePaths: lib.Extract.Exclusions
                    );
            } else {
                return new CopyArchInstallationOperation(
                    packageName: lib.Name,
                    packageVersion: archVersion,
                    cacheManager: cacheManager,
                    cachedName: cacheEntryName,
                    targetPath: $"libraries/{lib.Package}/{lib.Name}/{lib.Version}/{lib.Name}-{lib.Version}{nativeTagOrNull}.jar"
                    );
            }
        }
    }
}
