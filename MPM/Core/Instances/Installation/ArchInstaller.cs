using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Cache;
using MPM.Core.Instances.Installation.Scripts;
using MPM.Extensions;
using Platform.VirtualFileSystem;
using semver.tools;

namespace MPM.Core.Instances.Installation {

	public class ArchInstaller {

		private IFileMap ToFileMap(IEnumerable<ArchInstallationOperation> operations) {
			var fileMaps = operations.Select(op => op.GenerateOperations());
			return FileMap.MergeOrdered(fileMaps);
		}
	}

	public abstract class ArchInstallationOperation {
		public string PackageName { get; set; }
		public SemanticVersion PackageVersion { get; set; }
		public ICacheManager Cache { get; set; }

		public ArchInstallationOperation(string packageName, SemanticVersion packageVersion, ICacheManager cacheManager) {
			this.PackageName = packageName;
			this.Cache = cacheManager;
		}

		public abstract IFileMap GenerateOperations();
	}

	/// <summary>
	/// Extracts an entire cached archive, excluding any ignored paths, to a location
	/// </summary>
	internal class ExtractArchInstallationOperation : ArchInstallationOperation {

		//The targetted file or the entire targetted directory will be extracted where not starting with an ignored path.
		//Use "." to copy the entire archive.
		public string SourcePath { get; set; }

		//Directories will be merged, files will be overwritten.
		public string TargetPath { get; set; }

		public string CacheEntryName { get; set; }

		public IEnumerable<string> IgnorePaths { get; set; }

		public ExtractArchInstallationOperation(
				string packageName,
				SemanticVersion packageVersion,
				ICacheManager cacheManager,
				string cachedName,
				string sourcePath,
				string targetPath,
				IEnumerable<string> ignorePaths = null
			) : base(packageName, packageVersion, cacheManager) {
			this.IgnorePaths = ignorePaths.Denull();
			this.SourcePath = sourcePath;
			this.CacheEntryName = cachedName;
			this.TargetPath = targetPath;
		}

		public override IFileMap GenerateOperations() {
			//Iterate each path in the archive, building a filemap of extract operations to use.
			//TODO: Implement
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Extracts a single file from the specified cached archive to a location
	/// </summary>
	internal class ExtractSingleArchInstallationOperation : ArchInstallationOperation {
		public string CachedName { get; set; }
		public string SourcePath { get; set; }
		public string TargetPath { get; set; }

		public ExtractSingleArchInstallationOperation(
				string packageName,
				SemanticVersion packageVersion,
				ICacheManager cacheManager,
				string cachedName,
				string sourcePath,
				string targetPath
			) : base(packageName, packageVersion, cacheManager) {
			this.CachedName = cachedName;
			this.SourcePath = sourcePath;
			this.TargetPath = targetPath;
		}

		public override IFileMap GenerateOperations() {
			//Build an extract operation for the given archive that extracts the source.
			var fileMap = new FileMap();
			var exOp = new ExtractFileOperation(PackageName, PackageVersion, CachedName, SourcePath);
			fileMap.Register(TargetPath, exOp);
			return fileMap;
		}
	}

	internal class CopyArchInstallationOperation : ArchInstallationOperation {
		public string CachedName { get; set; }
		public string TargetPath { get; set; }

		public CopyArchInstallationOperation(
				string packageName,
				SemanticVersion packageVersion,
				ICacheManager cacheManager,
				string cachedName,
				string targetPath
			) : base(packageName, packageVersion, cacheManager) {
			this.CachedName = cachedName;
			this.TargetPath = targetPath;
		}

		public override IFileMap GenerateOperations() {
			//Build an copy operation for the given file that copies the source to the destination.
			var fileMap = new FileMap();
			var copyOp = new CopyFileOperation(PackageName, PackageVersion, CachedName);
			fileMap.Register(TargetPath, copyOp);
			return fileMap;
		}
	}
}
