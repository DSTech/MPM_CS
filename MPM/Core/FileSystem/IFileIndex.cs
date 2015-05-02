using System;
using System.Collections.Generic;
using MPM.Core.Instances;

namespace MPM.Core.FileSystem {
	public static class IFileIndexExtensions {
		/// <summary>
		/// Adds the effects of an <see cref="IInstallationOperation"/> to the tree.
		/// </summary>
		/// <param name="installationOperation">The operation to append to the tree</param>
		public static void Update(this IFileIndex fileIndex, string packageName, IInstallationOperation installationOperation) {
			throw new NotImplementedException();
		}
	}
	/// <summary>
	/// Stores the origin information for a file such as which package it came from, and where it should reside.
	/// Assumes that the consumed package can be identified by name.
	/// </summary>
	public interface IFileIndex {//TODO: Simplify to store only a multi-mapping of Package => FileDeclaration[]
		/// <summary>
		/// Calculates the difference between the file index and the root of the specified filesystem
		/// </summary>
		/// <param name="fileSystem"><see cref="IFileSystem"/> instance to differentiate against.</param>
		/// <returns></returns>
		IFileTreeDelta CalculateDelta(IFileSystem fileSystem);
		/// <summary>
		/// Creates a backup of the state of the instance.
		/// </summary>
		/// <returns><see cref="IFileTreeDelta"/> difference from an empty root to produce the state of the index.</returns>
		IFileTreeDelta Save();
		/// <summary>
		/// Restores a backup of the state of the instance.
		/// </summary>
		/// <param name="backup"><see cref="IFileTreeDelta"/> created by a call to <see cref="CalculateDelta(IFileSystem)"/>, or an instance differenced from an empty root.</param>
		void Restore(IFileTreeDelta backup);
	}
}
