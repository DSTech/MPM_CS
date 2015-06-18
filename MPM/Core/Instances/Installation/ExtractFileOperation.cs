using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Cache;
using Platform.VirtualFileSystem;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace MPM.Core.Instances.Installation {
	/// <summary>
	/// An operation to extract a file from a cached archive to a specified target.
	/// </summary>
	public class ExtractFileOperation : IFileOperation {
		public string ArchiveCacheEntry { get; set; }

		public string SourcePath { get; set; }

		public ExtractFileOperation() {
		}

		public ExtractFileOperation(string archiveCacheEntry, string sourcePath) {
			this.ArchiveCacheEntry = archiveCacheEntry;
			this.SourcePath = sourcePath;
		}

		public bool Reversible => false;

		public bool UsesPreviousContents => false;

		public void Perform(IFileSystem fileSystem, String path, ICacheReader cache) {
			var targetFile = fileSystem.ResolveFile(path);
			if (targetFile.Exists) {
				targetFile.Delete();
			}
			var cacheEntry = cache.Fetch(ArchiveCacheEntry);
			if (cacheEntry == null) {
				throw new KeyNotFoundException($"Cache did not contain entry for {ArchiveCacheEntry}");
			}
			using (var zip = new ZipInputStream(cacheEntry.FetchStream()) {
				IsStreamOwner = true,
			}) {
				ZipEntry entry;
				while ((entry = zip.GetNextEntry()) != null) {
					if (entry.Name != SourcePath) {
						continue;
					}
					using (var fileWriter = targetFile.GetContent().GetOutputStream(System.IO.FileMode.Create)) {
						zip.CopyTo(fileWriter);
					}
					break;
				}
			}
		}

		public void Reverse(IFileSystem fileSystem, String path, ICacheReader cache) {
			Debug.Assert(false);
			throw new NotSupportedException();
		}
	}
}
