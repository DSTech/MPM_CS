using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Platform.VirtualFileSystem;

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
}
