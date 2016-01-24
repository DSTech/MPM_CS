using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Cache;
using MPM.Core.Instances.Installation;
using MPM.Core.Protocols;
using MPM.Extensions;
using MPM.Net.Protocols.Minecraft.Types;
using Nito.AsyncEx.Synchronous;
using semver.tools;

namespace MPM.Net.Protocols.Minecraft {
    public class MinecraftArchInstaller {
        public IArchInstallationProcedure EnsureCached(SemanticVersion archVersion, ICacheManager cacheManager, IProtocolResolver protocolResolver) {
            var mdc = new MinecraftDownloadClient();
            var versionDetails = mdc.FetchVersion(archVersion.ToString()).WaitAndUnwrapException();
            Console.WriteLine(versionDetails.Id);
            var libsToInstall = versionDetails.Libraries.Where(lib => lib.Applies(Environment.OSVersion.Platform));
            var operations = new List<ArchInstallationOperation>();
            foreach (var lib in libsToInstall) {
                var appliedNatives = lib.ApplyNatives(Environment.OSVersion.Platform);
                var cacheEntryName = $"{lib.Package}_{lib.Name}_{lib.Version}{"_" + appliedNatives ?? "_u"}";
                var cacheOp = GenerateOp(archVersion, lib, appliedNatives, cacheEntryName, cacheManager);
                operations.Add(cacheOp);
                if (cacheManager.Contains(cacheEntryName)) {
                    Console.WriteLine($"File already cached as {cacheEntryName}.");
                    continue;
                }
                Console.WriteLine($"Fetching {lib.Name}...");
                var libStream = mdc.FetchLibrary(lib.Package, lib.Name, lib.Version, appliedNatives).WaitAndUnwrapException();
                Console.WriteLine($"Writing to cache {cacheEntryName}...");
                using (libStream) {
                    cacheManager.Store(cacheEntryName, libStream.ReadToEnd());
                }
            }
            return new ArchInstallationProcedure(operations.ToArray());
        }

        private ArchInstallationOperation GenerateOp(
            SemanticVersion archVersion,
            LibrarySpec lib,
            string appliedNatives,
            string cacheEntryName,
            ICacheManager cacheManager
            ) {
            var nativeTag = appliedNatives != null ? $"-{appliedNatives}" : null;
            if (lib.Extract != null) {
                //generate extract def
                string extractDestination = nativeTag != null ? "bin/natives/" : "bin/";
                return new ExtractArchInstallationOperation(
                    lib.Name,
                    archVersion,
                    cacheManager,
                    cacheEntryName,
                    "",
                    extractDestination,
                    lib.Extract.Exclusions
                    );
            } else {
                return new CopyArchInstallationOperation(
                    lib.Name,
                    archVersion,
                    cacheManager,
                    cacheEntryName,
                    $"libraries/{lib.Package}/{lib.Name}/{lib.Version}/{lib.Name}-{lib.Version}{nativeTag}.jar"
                    );
            }
        }
    }
}
