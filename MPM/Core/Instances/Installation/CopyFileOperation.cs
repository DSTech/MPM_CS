using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using MPM.Archival;
using MPM.Core.Instances.Cache;
using Platform.VirtualFileSystem;
using semver.tools;

namespace MPM.Core.Instances.Installation {

	/// <summary>
	/// An operation to extract a file from a cached archive to a specified target.
	/// </summary>
	public class CopyFileOperation : IFileOperation {
		public bool UsesPreviousContents => false;

		public string PackageName { get; set; }

		public SemanticVersion PackageVersion { get; set; }

		public string CacheEntryName { get; set; }

		public CopyFileOperation() {
		}

		public CopyFileOperation(string packageName, SemanticVersion packageVersion, string cacheEntryName) {
			this.PackageName = PackageName;
			this.PackageVersion = packageVersion;
			this.CacheEntryName = cacheEntryName;
		}

		public void Perform(IFileSystem fileSystem, String path, ICacheReader cache) {
			var targetFile = fileSystem.ResolveFile(path);
			if (targetFile.Exists) {
				targetFile.Delete();
			}
			var targetDir = targetFile.ResolveDirectory(".");
			if (!targetDir.Exists) {
				targetDir.Create(true);
			}
			var cacheEntry = cache.Fetch(CacheEntryName);
			if (cacheEntry == null) {
				throw new KeyNotFoundException($"Cache did not contain an entry for {CacheEntryName}");
			}
			using (var entryStream = cacheEntry.FetchStream())
			using (var fileWriter = targetFile.GetContent().GetOutputStream(System.IO.FileMode.Create)) {
				entryStream.CopyTo(fileWriter);
			}
		}

		public override string ToString() => $"<Copy {CacheEntryName}>";
	}
}
