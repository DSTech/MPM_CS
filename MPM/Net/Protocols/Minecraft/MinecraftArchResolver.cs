using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive.Disposables;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Cache;
using MPM.Core.Instances.Installation;
using MPM.Core.Protocols;
using MPM.Extensions;
using MPM.Net.Protocols.Minecraft.ProtocolTypes;
using MPM.Types;
using MPM.Util;
using Nito.AsyncEx.Synchronous;

namespace MPM.Net.Protocols.Minecraft {
    public class MinecraftArchInstaller {
        public IArchInstallationProcedure EnsureCached(MPM.Types.CompatibilitySide archSide, MPM.Types.SemVersion archVersion, ICacheManager cacheManager, IProtocolResolver protocolResolver) {
            var mdc = new MinecraftDownloadClient();
            MinecraftVersion versionDetails;
            {
                Console.WriteLine("Fetching minecraft details");
                var elapsedTime = TimerUtil.Time(out versionDetails, () => {
                    versionDetails = mdc.FetchVersion(archVersion);
                });
                Console.WriteLine("Fetched details for {0} in {1}ms", versionDetails.Id, elapsedTime.TotalMilliseconds);
            }
            var installingPlatform = Environment.OSVersion.Platform;
            var installingBitness64 = Environment.Is64BitOperatingSystem;

            Console.WriteLine("Installing client and server jars...");
            var operations = new List<ArchInstallationOperation>();
            operations.AddRange(CacheServerAndClientJars(archVersion, cacheManager, mdc, versionDetails));
            Console.WriteLine("Client and server jars installed.");

            var assetIndex = mdc.FetchAssetIndex(versionDetails);
            if (assetIndex.Assets.Count > 0) {
                Console.WriteLine("Caching and verifying assets...");
                operations.AddRange(CacheAssets(archVersion, assetIndex, mdc, cacheManager));
                Console.WriteLine("Assets cached.");
            }

            if (versionDetails.Libraries.Count > 0) {
                Console.WriteLine("Caching libraries...");
                operations.AddRange(CacheLibraries(archVersion, cacheManager, mdc, versionDetails, installingPlatform, installingBitness64));
                Console.WriteLine("Libraries cached.");
            }

            return new ArchInstallationProcedure(operations.ToArray());
        }

        private IEnumerable<ArchInstallationOperation> CacheLibraries(SemVersion archVersion, ICacheManager cacheManager, MinecraftDownloadClient mdc, MinecraftVersion versionDetails, PlatformID installingPlatform, bool installingBitness64) {
            var operations = new List<ArchInstallationOperation>();
            var libsToInstall = versionDetails.Libraries.Where(lib => lib.AppliesToPlatform(Environment.OSVersion.Platform));
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
                    continue;
                }
                Console.WriteLine($"Fetching {lib.Name}...");
                var libStream = mdc.FetchArtifact(nativeArtifact ?? lib.Downloads.Artifact).WaitAndUnwrapException();
                Console.WriteLine($"Writing to cache {cacheEntryName}...");
                using (libStream) {
                    cacheManager.Store(cacheEntryName, libStream.ReadToEnd());
                }
            }
            return operations;
        }

        private static IEnumerable<ArchInstallationOperation> CacheServerAndClientJars(SemVersion archVersion, ICacheManager cacheManager, MinecraftDownloadClient mdc, MinecraftVersion versionDetails) {
            var operations = new List<ArchInstallationOperation>(2);
            {
                var binaryCacheClientName = $"minecraft_{archVersion}_client.jar";
                if (!cacheManager.Contains(binaryCacheClientName)) {
                    var clientBinaryStream = mdc.FetchClient(versionDetails).WaitAndUnwrapException();
                    cacheManager.StoreFromStream(binaryCacheClientName, clientBinaryStream);
                }
                var clientBinaryCopyOp = new CopyArchInstallationOperation("minecraft", archVersion, cacheManager, binaryCacheClientName, "client.jar");
                operations.Add(clientBinaryCopyOp);
            }
            {
                var binaryCacheServerName = $"minecraft_{archVersion}_server.jar";
                if (!cacheManager.Contains(binaryCacheServerName)) {
                    var serverBinaryStream = mdc.FetchServer(versionDetails).WaitAndUnwrapException();
                    cacheManager.StoreFromStream(binaryCacheServerName, serverBinaryStream);
                }
                var serverBinaryCopyOp = new CopyArchInstallationOperation("minecraft", archVersion, cacheManager, binaryCacheServerName, "server.jar");
                operations.Add(serverBinaryCopyOp);
            }
            return operations;
        }

        private ArchInstallationOperation GenerateOp(SemVersion archVersion, LibrarySpec lib, ArtifactSpec artifact, string nativeTagOrNull, string cacheEntryName, ICacheManager cacheManager) {
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
                    targetPath: $"libraries/{lib.Package.Replace('.', '/')}/{lib.Name}/{lib.Version}/{lib.Name}-{lib.Version}{nativeTagOrNull}.jar"
                    );
            }
        }


        private struct AssetCacheEntry {
            public string CacheId { get; set; }
            public Asset Asset { get; set; }
        }

        private static IEnumerable<ArchInstallationOperation> CacheAssets(SemVersion archVersion, AssetIndex assetIndex, MinecraftDownloadClient mdc, ICacheManager cacheManager) {
            var assetsWithIds = assetIndex.Assets.Select(asset => new AssetCacheEntry {
                CacheId = $"arch/minecraft/{archVersion}/asset/{asset.Uri.ToString()}",
                Asset = asset,
            }).ToArray();

            using (var sha1 = new SHA1Managed()) {
                var assetsToDownload = new List<AssetCacheEntry>();
                //Check currently available assets- Check cached values, flag bad entries for redownload
                foreach (var _asset in assetsWithIds) {
                    var asset = _asset.Asset;
                    var assetCacheId = _asset.CacheId;
                    if (!cacheManager.Contains(assetCacheId)) {
                        assetsToDownload.Add(_asset);
                        continue;
                    }
                    var data = cacheManager.Fetch(assetCacheId).Fetch();
                    if (data.LongLength != asset.Size) {
                        Console.WriteLine($"Cached asset {asset.Uri} with size {data.LongLength} did not match size {asset.Size}, redownloading.");
                        cacheManager.Delete(assetCacheId);
                        assetsToDownload.Add(_asset);
                        continue;
                    }
                    var cachedAssetHash = new Hash("sha1", sha1.ComputeHash(data));
                    if (cachedAssetHash != asset.Hash) {
                        Console.WriteLine($"Cached asset {asset.Uri} with hash {cachedAssetHash} did not match hash {asset.Hash}, redownloading.");
                        cacheManager.Delete(assetCacheId);
                        assetsToDownload.Add(_asset);
                        continue;
                    }
                    //Entry is okay, continue
                }

                //Download missing assets to cache
                {
                    var currentIndex = 0;
                    var assetCount = assetsToDownload.Count;
                    if (assetCount > 0) {
                        var assetCountCharWidth = assetCount.ToString().Length;
                        var totalSize = assetsToDownload.Sum(asset => asset.Asset.Size);
                        Console.WriteLine($"Downloading {assetCount} files adding up to {Huminz.ByteSize(totalSize)}");
                        foreach (var _asset in assetsToDownload) {//C# needs real destructuring support
                            var assetCacheId = _asset.CacheId;
                            var asset = _asset.Asset;

                            var countSection = $"[{(++currentIndex).ToString().PadLeft(assetCountCharWidth, '0')}/{assetCount}]";
                            var byteLength = $"({Huminz.ByteSizeShort(asset.Size).PadLeft(8)})";

                            Console.WriteLine($"\t{countSection} {byteLength} {asset.Uri}");

                            var assetStream = mdc.FetchAsset(asset).WaitAndUnwrapException();
                            var assetData = assetStream.ReadToEndAndClose();
                            if (assetData.LongLength != asset.Size) {
                                throw new Exception($"Error downloading asset {asset.Uri} with size {assetData.LongLength} did not match size {asset.Size}!");
                            }
                            var downloadedAssetHash = new Hash("sha1", sha1.ComputeHash(assetData));
                            if (downloadedAssetHash != asset.Hash) {
                                throw new Exception($"Error downloading asset {asset.Uri}:\n\t{downloadedAssetHash} did not match hash {asset.Hash}!");
                            }
                            cacheManager.Store(assetCacheId, assetData);
                        }
                    }
                }
            }

            var operations = new List<ArchInstallationOperation>();
            //Install all entries
            foreach (var _asset in assetsWithIds) {
                var hexHash = Hex.GetString(_asset.Asset.Hash.Checksum);
                var assetCopy = new CopyArchInstallationOperation(
                    packageName: "minecraft",
                    packageVersion: archVersion,
                    cacheManager: cacheManager,
                    cachedName: _asset.CacheId,
                    targetPath: $"assets/objects/{hexHash.Substring(0, 2)}/{hexHash}"
                    );
                //Milestone Gamma: 1.7.2 and below require files to be stored in a different format under assets/virtual/legacy/
                operations.Add(assetCopy);
            }
            return operations;
        }
    }
}
