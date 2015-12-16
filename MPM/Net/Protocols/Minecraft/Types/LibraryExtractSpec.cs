using System.Collections.Generic;
using System.Linq;
using MPM.Extensions;

namespace MPM.Net.Protocols.Minecraft.Types {

	public class LibraryExtractSpec {
		public LibraryExtractSpec() {
		}
		public LibraryExtractSpec(IEnumerable<string> exclusionPaths = null) {
			Exclusions = exclusionPaths.Denull().ToList();
		}

		/// <summary>
		/// A list of paths to exclude from extraction when installing the library
		/// </summary>
		public List<string> Exclusions { get; set; }
	}
}
