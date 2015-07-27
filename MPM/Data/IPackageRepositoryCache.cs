using System.Threading.Tasks;

namespace MPM.Data {

	/// <summary>
	/// Provides a wrapper around <see cref="IPackageRepository"/> with which the calls may be cached to reduce repository load and increase responsiveness.
	/// </summary>
	public interface IPackageRepositoryCache : IPackageRepository {

		/// <summary>
		/// Should be called at least once before usage, unless the particular implementation has loaded information from another source.
		/// Provides a means for the cache to retreive any changed package information from the repository.
		/// It should be assumed that the longer the duration since the last call to this function is, the less accurate the cache may be to the current state of the repository.
		/// </summary>
		Task Sync();
	}
}
