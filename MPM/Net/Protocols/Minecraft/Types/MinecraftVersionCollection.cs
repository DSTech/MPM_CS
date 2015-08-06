using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Extensions;

namespace MPM.Net.Protocols.Minecraft.Types {

	public class MinecraftVersionCollection : IReadOnlyList<MinecraftVersionListing> {

		public MinecraftVersionCollection(MinecraftVersionListing latestRelease, MinecraftVersionListing latestSnapshot, IEnumerable<MinecraftVersionListing> versions) {
			this.LatestRelease = latestRelease;
			this.LatestSnapshot = latestSnapshot;
			this.Versions = versions.Denull().ToArray();
		}

		public MinecraftVersionListing LatestSnapshot { get; }
		public MinecraftVersionListing LatestRelease { get; }

		public IReadOnlyList<MinecraftVersionListing> Versions { get; }

		public int Count => Versions.Count;

		public MinecraftVersionListing this[int index] => Versions[index];

		public IEnumerator<MinecraftVersionListing> GetEnumerator() {
			return Versions.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return Versions.GetEnumerator();
		}
	}
}
