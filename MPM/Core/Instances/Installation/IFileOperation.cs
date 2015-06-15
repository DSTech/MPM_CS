using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MPM.Core.FileSystem;

namespace MPM.Core.Instances.Installation {
	public static class IFileOperationExtensions {
		/// <summary>
		/// Reduces a sequence of operations to the minimal set that does not ignore existing contents.
		/// </summary>
		/// <param name="opChain">Chain to reduce.</param>
		/// <returns>
		/// A simplified chain of operations, containing at most one operation that does not fulfill <see cref="UsesPreviousContents(IFileOperation)"/>.
		/// </returns>
		public static IEnumerable<IFileOperation> Cull(this IEnumerable<IFileOperation> opChain) {
			var store = new Queue<IFileOperation>();
			foreach (var op in opChain) {
				if (!op.UsesPreviousContents) {
					store.Clear();
				}
				store.Enqueue(op);
			}
			//TODO: Clear the last operation if it is a deletion. Empty chains are a no-op.
			return store.ToArray();
		}
		public static bool IsReversible(this IEnumerable<IFileOperation> opChain) {
			return opChain.All(entry => entry.Reversible && entry.UsesPreviousContents);
		}
		/// <summary>
		/// Creates an <see cref="IEnumerable{IFileOperation}"/> which will revert an <paramref name="opChain"/>
		/// such that the <paramref name="lastOp"/> will be the last operation that occurred on the target.
		/// Will create a hard-reverse sequence if necessary steps were not reversible.
		/// </summary>
		/// <param name="opChain">The chain of operations that brought the file to its current state.</param>
		/// <param name="lastOp">The last operation that should appear to have been performed on the file.</param>
		/// <returns>A <see cref="IEnumerable{IFileOperation}"/> that, when performed in order, results in a file of the requested state.</returns>
		public static IEnumerable<IFileOperation> CreateReversionTo(this IEnumerable<IFileOperation> opChain, IFileOperation lastOp) {
			var _opChain = opChain.ToList();
			var lastIndex = _opChain.LastIndexOf(lastOp);
			if (lastIndex < 0) {
				throw new ArgumentOutOfRangeException(nameof(lastOp), $"was not in {nameof(opChain)}");
			}
			var stepsAfter = _opChain.SubEnumerable(lastIndex + 1);
			if (stepsAfter.IsReversible()) {
				Debug.Assert(stepsAfter.All(s => s.UsesPreviousContents));
				return stepsAfter.Reverse().ToArray();
			}
			return _opChain.Take(lastIndex + 1).ToArray();
		}
	}
	//TODO: Determine which resource-providing interfaces should be accessible in order to perform operations, eg means of fetching files from repositories
	//	At minimum, likely Cache for both perform and reverse, and Network for perform.
	//	Contemplation: Should Network be provided to perform at all?
	//		"Yes" seems to be the most sane option, being that some operations will be extraction or downloading...
	//	Contemplation: What if the operation just stores the instruction it came from, and caching is provided elsewhere, with the needed files provided by ... What exactly?
	//		Maybe the InstallationOperation entry can provide resources through a request interface, which handles the logistics?
	//		This is the chosen path. InstallationOperations will be required to cache anything required for performing operations,
	//			then register determined operations to the filemap.
	//			This should allow extractions to register all produced filemap entries from the extracted contents.
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
		/// <param name="uri">The path of the content which should be altered.</param>
		void Perform(IFileSystem fileSystem, Uri uri);
		/// <summary>
		/// Reverts the actions of the operation on the specified path within the given filesystem.
		/// </summary>
		/// <remarks>
		/// Reversing of operations may not request resources from the network, only from the cache.
		/// </remarks>
		/// <param name="fileSystem">The filesystem upon which the operations should be reversed.</param>
		/// <param name="uri">The path of the content which should be altered.</param>
		/// <exception cref="NotSupportedException">
		/// Must be thrown when called upon a non-reversible instance.
		/// </exception>
		void Reverse(IFileSystem fileSystem, Uri uri);
	}
}
