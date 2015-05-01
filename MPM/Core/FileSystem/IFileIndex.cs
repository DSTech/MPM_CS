namespace MPM.Core.FileSystem {
	public interface IFileIndex {
		/// <summary>
		/// Calculates the difference between the file index and the root of the specified virtual filesystem
		/// </summary>
		/// <param name="fileSystem"></param>
		/// <returns></returns>
		IFileTreeDelta CalculateDelta(IFileSystem fileSystem);
	}
}
