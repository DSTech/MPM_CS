using System;
using System.Collections.Generic;
using System.Linq;

namespace MPM.Core.FileSystem {
	public static class IFileTreeDeltaExtensions {
		private class CombinedFileTree : IFileTreeDelta {
			public IEnumerable<IFileDelta> Entries {
				get;
				set;
			}
		}
		/// <summary>
		/// Naive implementation allowing combination of two ordered <see cref="IFileTreeDelta"/> instances into a single instance.
		/// </summary>
		/// <remarks>
		/// This would likely be more useful given a reducing mechanism which collapses operations such as
		/// add->delete and move(->edit?)->move into single operations to optimize the combined result.
		/// </remarks>
		/// <param name="first">First delta to consider</param>
		/// <param name="second">Delta to stack upon the first</param>
		/// <returns></returns>
		public static IFileTreeDelta Combine(this IFileTreeDelta first, IFileTreeDelta second) {
			return new CombinedFileTree {
				Entries = Enumerable.Concat(first.Entries, second.Entries).ToArray(),
			};
		}
	}
	/// <summary>
	/// Serves to describe a change between one set of files and another.
	/// A basic set of files (Non-delta) can be described by an instance with only file creation entries.
	/// Directories are implicit, and should be assumed to be created or destroyed when files
	/// do or do not exist to occupy them, in similar behavior to Git.
	/// Entries are order-dependent.
	/// </summary>
	public interface IFileTreeDelta {
		IEnumerable<IFileDelta> Entries { get; }
	}
}
