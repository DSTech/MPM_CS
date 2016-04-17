using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    public class MinecraftVersionCollection : IReadOnlyList<MinecraftVersionListing> {
        public MinecraftVersionCollection() {
        }

        public MinecraftVersionCollection(MinecraftVersionListing latestRelease, MinecraftVersionListing latestSnapshot, IEnumerable<MinecraftVersionListing> versions) {
            this.LatestRelease = latestRelease;
            this.LatestSnapshot = latestSnapshot;
            this.Versions = versions.Denull().ToList();
        }

        public MinecraftVersionListing LatestSnapshot { get; set; }
        public MinecraftVersionListing LatestRelease { get; set; }

        public List<MinecraftVersionListing> Versions { get; set; }

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
