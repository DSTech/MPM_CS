using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using MPM.Archival;
using MPM.Core.Instances.Cache;
using MPM.Core.Instances.Info;
using MPM.Core.Instances.Installation;
using MPM.Core.Instances.Installation.Scripts;
using MPM.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Platform.VirtualFileSystem;
using semver.tools;
using Xunit;

namespace MPMTest {
    public class InstallationScriptTests {
        private JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings() {
            TypeNameHandling = TypeNameHandling.Auto,
        };

        private JsonSerializer Serializer => JsonSerializer.Create(_jsonSerializerSettings);
        private string _testCachePath => Path.GetFullPath("./testCache");

        public string TestPackageName => "testPackage";
        public SemanticVersion TestPackageVersion => SemanticVersion.Parse("0.0.0");

        private ICacheManager FetchTestCache() {
            if (!Directory.Exists(_testCachePath)) {
                Directory.CreateDirectory(_testCachePath);
            }
            return new FileSystemCacheManager(_testCachePath);
        }

        [Fact]
        public void ParseScript() {
            ScriptFileDeclaration[] instructionArr;
            using (var testPackage = new JsonTextReader(new StreamReader(File.OpenRead(Path.Combine(".", "TestResources", "testPackage.json")))) { CloseInput = true }) {
                instructionArr = Serializer.Deserialize<RawBuildInfo>(testPackage).Installation;
            }
            var installationScript = instructionArr.Select(instruction => instruction.Parse(TestPackageName, TestPackageVersion)).ToArray();
            //Console.WriteLine($"Installation Script (#{installationScript.Length}) =>");
            //foreach (var entry in installationScript) Console.WriteLine("  {0}", entry.ToString().Replace("\n", "\n  "));
        }

        /// <summary>
        ///     Verifies that zip paths are formatted in the style "dir/dir/file.ext"
        /// </summary>
        [Fact]
        public void ZipURIs() {
            List<String> entryNames = new List<String>();
            using (var zip = new ZipInputStream(File.OpenRead(Path.Combine(".", "TestResources", "testZipFile.zip")))) {
                ZipEntry entry;
                while ((entry = zip.GetNextEntry()) != null) {
                    entryNames.Add(entry.Name);
                }
            }
            Assert.Contains(entryNames, t => t == "testFile");
            Assert.Contains(entryNames, t => t == "testDir/testFile2");
            Assert.True(entryNames.Count == 2);
        }

        /// <summary>
        ///     Builds a testing package and saves it into the specified cache, returning the cached entry name.
        /// </summary>
        /// <param name="cache">The cache in which to save the package</param>
        /// <returns>Cache entry name</returns>
        private String CacheTestPackage(ICacheManager cache, string packageName, SemanticVersion packageVersion) {
            byte[] testPackage;
            using (var memStr = new MemoryStream()) {
                using (var zip = new ZipOutputStream(memStr) { IsStreamOwner = false }) {
                    foreach (var pair in new Dictionary<String, String> {
                        ["testPackage.json"] = "package.json",
                        ["testConfig.cfg"] = "testConfig.cfg",
                        ["testText.txt"] = "testText.txt",
                    }) {
                        var fileContents = File.ReadAllBytes(Path.Combine(".", "TestResources", pair.Key));
                        var zipEntry = new ZipEntry(pair.Value);
                        zipEntry.Size = fileContents.Length;
                        zip.PutNextEntry(zipEntry);
                        zip.Write(fileContents, 0, fileContents.Length);
                        zip.CloseEntry();
                    }
                }
                testPackage = memStr.ToArray();
            }
            var keyName = $"testPackage_{new Random().Next()}.zip";
            cache.Store(keyName, testPackage);
            return keyName;
        }

        [Fact]
        public void CanBuildPackageFile() {
            var testCache = FetchTestCache();
            var testPackageCacheEntry = CacheTestPackage(testCache, TestPackageName, TestPackageVersion);
            try {
                Assert.True(testCache.Contains(testPackageCacheEntry));
            } finally {
                testCache.Delete(testPackageCacheEntry);
            }
        }

        [Fact]
        public void CanPerformExtractionOp() {
            var testCache = FetchTestCache();
            var testPackageCacheEntry = CacheTestPackage(testCache, TestPackageName, TestPackageVersion);
            try {
                Assert.True(testCache.Contains(testPackageCacheEntry));
                var exOp = new ExtractFileOperation(TestPackageName, TestPackageVersion, testPackageCacheEntry, "package.json");
                var testFsPath = Path.GetFullPath(Path.Combine(".", "testFs"));
                if (!Directory.Exists(testFsPath)) {
                    Directory.CreateDirectory(testFsPath);
                }
                var testTargetName = $"testTarget_{new Random().Next()}.json";
                try {
                    using (var testFs = FileSystemManager.Default.ResolveDirectory(testFsPath).CreateView()) {
                        exOp.Perform(testFs, testTargetName, testCache);
                    }
                    Assert.True(File.Exists(Path.Combine(testFsPath, testTargetName)));
                } finally {
                    File.Delete(Path.Combine(testFsPath, testTargetName));
                }
            } finally {
                testCache.Delete(testPackageCacheEntry);
            }
        }

        [Fact]
        public void CanPerformBasicInstallation() {
            var testCache = FetchTestCache();
            var testPackageCacheEntry = CacheTestPackage(testCache, TestPackageName, TestPackageVersion);
            try {
                Assert.True(testCache.Contains(testPackageCacheEntry));
                var exOp = new ExtractFileOperation(TestPackageName, TestPackageVersion, testPackageCacheEntry, "testText.txt");
                var testFsPath = Path.GetFullPath(Path.Combine(".", "testFs"));
                if (!Directory.Exists(testFsPath)) {
                    Directory.CreateDirectory(testFsPath);
                }
                using (var testFs = FileSystemManager.Default.ResolveDirectory(testFsPath).CreateView()) {
                    RawBuildInfo packageInfo;
                    using (var zip = new SeekingZipFetcher(testCache.Fetch(testPackageCacheEntry).FetchStream())) {
                        var packageJsonBytes = zip.FetchFile("package.json");
                        var packageJsonString = Encoding.UTF8.GetString(packageJsonBytes);
                        packageInfo = JsonConvert.DeserializeObject<RawBuildInfo>(packageJsonString);
                    }

                    var installationScript = packageInfo.Installation.Select(decl => decl.Parse(packageInfo.Package, packageInfo.Version.FromDTO())).ToArray();

                    Console.WriteLine(packageInfo.Package);
                    Console.WriteLine(packageInfo.Version);
                    Console.WriteLine(packageInfo.GivenVersion);

                    var operationSets = installationScript
                        .Select(decl => {
                            Console.WriteLine(decl);
                            decl.EnsureCached(testPackageCacheEntry, testCache, protocolResolver: null);//TODO: Test protocol resolvers, once they're implemented
                            return decl.GenerateOperations();
                        }).ToArray();

                    var fileMap = FileMap.AsMergedFileMap(operationSets);

                    foreach (var file in fileMap) {
                        var fileName = file.Key;
                        var fileOps = file.Value;
                        foreach (var op in fileOps) {
                            op.Perform(testFs, fileName, testCache);
                        }
                    }

                    var testTextPath = "./mods/testing/testText.txt";
                    var testConfigPath = "./config/testing/testConfig.cfg";
                    try {
                        Assert.True(testFs.ResolveFile(testTextPath).Exists);
                        Assert.True(testFs.ResolveFile(testConfigPath).Exists);
                    } finally {
                        testFs.ResolveFile(testTextPath).Delete();
                        testFs.ResolveFile(testConfigPath).Delete();
                        testFs.ResolveDirectory("./mods").Delete(true);
                        testFs.ResolveDirectory("./config").Delete(true);
                    }
                }
            } finally {
                testCache.Delete(testPackageCacheEntry);
            }
        }
    }
}
