using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using MPM.Core.Instances.Cache;
using MPM.Core.Instances.Info;
using MPM.Core.Instances.Installation.Scripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using semver.tools;
using Xunit;

namespace MPMTest {
	public class InstallationScriptTests {
		private JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings() {
			TypeNameHandling = TypeNameHandling.Auto,
		};
		private JsonSerializer Serializer => JsonSerializer.Create(_jsonSerializerSettings);
		private StreamReader FetchTestScript() {
			return new StreamReader(File.OpenRead(Path.Combine(".", "TestResources", "InstallationScript.json")));
		}
		private string _testCachePath => Path.GetFullPath("./testCache");
		private ICacheManager FetchTestCache() {
			if (!Directory.Exists(_testCachePath)) {
				Directory.CreateDirectory(_testCachePath);
			}
			return new FileSystemCacheManager(_testCachePath);
		}
		public string TestPackageName => "testPackage";
		public SemanticVersion TestPackageVersion => SemanticVersion.Parse("0.0.0");

		[Fact]
		public void ParseScript() {
			ScriptFileDeclaration[] instructionArr;
			using (var testScript = new JsonTextReader(FetchTestScript()) { CloseInput = true }) {
				instructionArr = Serializer.Deserialize<IEnumerable<ScriptFileDeclaration>>(testScript).ToArray();
			}
			Console.WriteLine($"Found {instructionArr.Length} instructions...");
			var installationScript = instructionArr.Select(instruction => instruction.Parse(TestPackageName, TestPackageVersion)).ToArray();
			Console.WriteLine($"Installation Script (#{installationScript.Length}) =>");
			foreach (var entry in installationScript) {
				Console.WriteLine("  {0}", entry.ToString().Replace("\n", "\n  "));
			}
		}

		/// <summary>
		/// Builds a testing package and saves it into the specified cache, returning the cached entry name.
		/// </summary>
		/// <param name="cache">The cache in which to save the package</param>
		/// <returns>Cache entry name</returns>
		private String CacheTestPackage(ICacheManager cache, string packageName, SemanticVersion packageVersion) {
			throw new NotImplementedException();
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

		/// <summary>
		/// Verifies that zip paths are formatted in the style "dir/dir/file.ext"
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
	}
}
