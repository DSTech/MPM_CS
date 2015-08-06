using System.Collections.Generic;
using MPM.Extensions;

namespace MPM.Net.Protocols.Minecraft.Types {

	public class LibraryExtractSpec {

		public LibraryExtractSpec(IEnumerable<string> exclusionPaths = null) {
			exclusionPaths = exclusionPaths.Denull();
		}

		/// <summary>
		/// A list of paths to exclude from extraction when installing the library
		/// </summary>
		public IReadOnlyList<string> Exclusions { get; }
	}
}
