using System;
using System.IO;

namespace MPM.Core.FileSystem {
	public class LocalFileNode : IFileNode {
		private LocalFileSystem localFileSystem { get; }
		public Uri Location { get; }
		private string LocalPath {
			get {
				return localFileSystem.ToLocal(Location);
			}
		}

		public LocalFileNode(LocalFileSystem localFileSystem, Uri location) {
			if (localFileSystem == null) {
				throw new ArgumentNullException(nameof(localFileSystem));
			}
			this.localFileSystem = localFileSystem;
			if (location == null) {
				throw new ArgumentNullException(nameof(location));
			}
			this.Location = location;
		}

		public bool Exists {
			get {
				return File.Exists(LocalPath);
			}
		}

		public IFileSystem FileSystem {
			get {
				return localFileSystem;
			}
		}

		public bool Delete() {
			var localPath = LocalPath;
			if (!File.Exists(localPath)) {
				return false;
			}
			File.Delete(localPath);
			return true;
		}

		public Stream OpenCreate() {
			return File.Open(LocalPath, FileMode.Create, FileAccess.ReadWrite);
		}

		public Stream OpenCreateOrEdit() {
			return File.Open(LocalPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		}

		public Stream OpenEdit() {
			return File.Open(LocalPath, FileMode.Open, FileAccess.ReadWrite);
		}
	}
}
