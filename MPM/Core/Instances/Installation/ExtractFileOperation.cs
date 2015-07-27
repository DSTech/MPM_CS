using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using MPM.Core.Instances.Cache;
using Platform.VirtualFileSystem;
using semver.tools;

namespace MPM.Core.Instances.Installation {

	/// <summary>
	/// An operation to extract a file from a cached archive to a specified target.
	/// </summary>
	public class ExtractFileOperation : IFileOperation {
		public bool UsesPreviousContents => false;

		public string PackageName { get; set; }

		public SemanticVersion PackageVersion { get; set; }

		public string ArchiveCacheEntry { get; set; }

		public string SourcePath { get; set; }

		public ExtractFileOperation() {
		}

		public ExtractFileOperation(string packageName, SemanticVersion packageVersion, string archiveCacheEntry, string sourcePath) {
			this.PackageName = PackageName;
			this.PackageVersion = packageVersion;
			this.ArchiveCacheEntry = archiveCacheEntry;
			this.SourcePath = sourcePath;
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
			var cacheEntry = cache.Fetch(ArchiveCacheEntry);
			if (cacheEntry == null) {
				throw new KeyNotFoundException($"Cache did not contain an entry for {ArchiveCacheEntry}");
			}
			var entryStream = cacheEntry.FetchStream();
			if (entryStream.CanSeek) {
				using (var fileWriter = targetFile.GetContent().GetOutputStream(System.IO.FileMode.Create)) {
					using (var seeker = new SeekingZipFetcher(entryStream)) {
						using (var zipStream = seeker.FetchFileStream(SourcePath)) {
							zipStream.CopyTo(fileWriter);
						}
					}
				}
			} else {
				using (var zip = new StreamingZipFetcher(entryStream)) {
					using (var fileWriter = targetFile.GetContent().GetOutputStream(System.IO.FileMode.Create)) {
						var sourceFile = zip.FetchFile(SourcePath);
						fileWriter.Write(sourceFile, 0, sourceFile.Length);
					}
				}
			}
		}
	}
}
