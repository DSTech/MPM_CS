using System;

namespace MPM.Core.FileSystem {
	public class LocalFileSystem : IFileSystem {
		public LocalFileSystem(Uri root) {
			this.Root = root;
		}
		public Uri Root { get; }
		/// <summary>
		/// Converts a <see cref="Uri"/> relative to the <see cref="Root"/> for use in native functions.
		/// </summary>
		/// <param name="uri">Uri relative to <see cref="Root"/></param>
		/// <returns>A <see cref="string"/> path to the resource as identified by the local filesystem.</returns>
		public string ToLocal(Uri uri) {
			return new Uri(Root, uri).LocalPath;
		}
		public IFileNode Resolve(Uri location) {
			return new LocalFileNode(this, location);
		}
	}
}
