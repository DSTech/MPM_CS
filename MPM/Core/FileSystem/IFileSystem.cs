namespace MPM.Core.FileSystem {
	public interface IFileSystem {
		void Apply(IFileTreeDelta delta);
	}
}
