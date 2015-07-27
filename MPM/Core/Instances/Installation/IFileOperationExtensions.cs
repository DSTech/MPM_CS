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
				if (op is DeleteFileOperation || op is ExtractFileOperation) {//Operations which replace the content of the file
					store.Clear();//Remove previous operations, they will be replaced by their subsequent operations.
				}
				store.Enqueue(op);
			}
			var arr = store.ToArray();
			//If the last operation is a deletion, nothing should be performed.
			if (arr.Last() is DeleteFileOperation) {
				return new IFileOperation[0];
			}
			return arr;
		}
	}
}
