using System;
using System.Collections.Generic;
using System.Linq;
using MPM.Core.FileSystem;

namespace MPM.Core.Instances.Installation {
	public static class IFileOperationExtensions {
		/// <summary>
		/// Reduces a sequence of operations past the last deletion or replacement step that occurs.
		/// </summary>
		/// <param name="opChain">Chain to reduce.</param>
		/// <returns>A simplified chain of operations.</returns>
		public static IEnumerable<IFileOperation> Cull(this IEnumerable<IFileOperation> opChain) {
			throw new NotImplementedException();
		}
		public static bool IsReversible(this IEnumerable<IFileOperation> opChain) {
			return opChain.All(entry => entry.Reversible);
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
				return stepsAfter.Reverse().ToArray();
			}
			return _opChain.Take(lastIndex + 1).ToArray();
		}
	}
	//TODO: Determine which resource-providing interfaces should be accessible in order to perform operations, eg means of fetching files from repositories
	/// <summary>
	/// A single operation upon a specific filepath.
	/// </summary>
	/// <remarks>
	/// Lossy operations must not be reversible.
	/// </remarks>
	public interface IFileOperation {
		bool Reversible { get; }
		void Perform(IFileSystem fileSystem, Uri uri);
		void Reverse(IFileSystem fileSystem, Uri uri);
	}
}
