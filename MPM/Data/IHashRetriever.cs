using System.IO;
using System.Threading.Tasks;

namespace MPM.Data {

	/// <summary>
	/// A means of attaining a particular hash. May contain multiple redundant methods, similar to those provided by Metalinks or Magnet URIs in other systems.
	/// </summary>
	public interface IHashRetriever {

		/// <summary>
		/// The hash that will be retrieved by this instance.
		/// </summary>
		byte[] Hash { get; }

		Task<byte[]> Retrieve();

		/// <summary>
		/// Allows for stream-optimized methods of access where they may be considered more sensible.
		/// </summary>
		/// <returns>Stream of the information which must be disposed after usage.</returns>
		Task<Stream> RetrieveStream();
	}
}
