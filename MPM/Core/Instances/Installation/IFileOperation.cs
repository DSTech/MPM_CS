using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MPM.Core.Instances.Cache;
using Platform.VirtualFileSystem;

namespace MPM.Core.Instances.Installation {
	/// <summary>
	/// A single operation upon a specific filepath.
	/// </summary>
	/// <remarks>
	/// Lossy operations must not be reversible.
	/// </remarks>
	public interface IFileOperation {
		/// <summary>
		/// Whether or not an operation uses the previous contents of a file if they are available.
		/// Is false if the operation ignores or destroys pre-existing contents, otherwise true.
		/// </summary>
		bool UsesPreviousContents { get; }
		/// <summary>
		/// Is the step reversible with only requests to the cache, or must the content be rebuilt from the beginning?
		/// </summary>
		bool Reversible { get; }
		/// <summary>
		/// Performs the operation upon the specified path within the given filesystem.
		/// </summary>
		/// <remarks>
		/// Reversible entries must have <see cref="UsesPreviousContents"/> as true.
		/// </remarks>
		/// <param name="fileSystem">The filesystem upon which the operations should be performed.</param>
		/// <param name="path">The path of the content which should be altered.</param>
		void Perform(IFileSystem fileSystem, String path, ICacheReader cache);
		/// <summary>
		/// Reverts the actions of the operation on the specified path within the given filesystem.
		/// </summary>
		/// <remarks>
		/// Reversing of operations may not request resources from the network, only from the cache.
		/// </remarks>
		/// <param name="fileSystem">The filesystem upon which the operations should be reversed.</param>
		/// <param name="path">The path of the content which should be altered.</param>
		/// <exception cref="NotSupportedException">
		/// Must be thrown when called upon a non-reversible instance.
		/// </exception>
		void Reverse(IFileSystem fileSystem, String path, ICacheReader cache);
	}
}
