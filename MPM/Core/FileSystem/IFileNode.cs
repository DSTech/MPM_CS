using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.FileSystem {
	public interface IFileNode {
		IFileSystem @FileSystem { get; }
		/// <summary>
		/// A URI relative to the retrieving filesystem.
		/// </summary>
		Uri Location { get; }
		/// <summary>
		/// Checks if the file at the specified <see cref="Location"/> exists.
		/// </summary>
		bool Exists { get; }
		/// <summary>
		/// Creates a new file, overwriting any existing file in the node's location.
		/// </summary>
		/// <returns>A <see cref="Stream"/> to the file which must be disposed to close the file.</returns>
		Stream OpenCreate();
		/// <summary>
		/// Opens the file if it exists, or creates and opens it if it does not.
		/// </summary>
		/// <returns>A <see cref="Stream"/> to the file which must be disposed to close the file.</returns>
		Stream OpenCreateOrEdit();
		/// <summary>
		/// Opens an existing file for editing.
		/// </summary>
		/// <exception cref="FileNotFoundException">Thrown if the specified file does not exist.</exception>
		/// <returns>A <see cref="Stream"/> to the file which must be disposed to close the file.</returns>
		Stream OpenEdit();
		/// <summary>
		/// Deletes the file at the specified location.
		/// </summary>
		/// <returns>Whether or not the specified file existed to be deleted.</returns>
		bool Delete();
	}
}
