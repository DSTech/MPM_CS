using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Net.Protocols.Minecraft.DTO {
    public class MinecraftVersionLatestSpec {
        public string Snapshot { get; set; }
        public string Release { get; set; }
    }

    [Newtonsoft.Json.JsonObject]
    public class MinecraftVersionCollection : ICollection<MinecraftVersionListing> {
        public MinecraftVersionLatestSpec Latest { get; set; } = new MinecraftVersionLatestSpec {
            Release = null,
            Snapshot = null,
        };

        public MinecraftVersionListing[] Versions { get; set; } = new MinecraftVersionListing[0];

        public int Count {
            get { return ((ICollection<MinecraftVersionListing>) Versions).Count; }
        }

        public bool IsReadOnly {
            get { return ((ICollection<MinecraftVersionListing>) Versions).IsReadOnly; }
        }

        public void Add(MinecraftVersionListing item) {
            ((ICollection<MinecraftVersionListing>) Versions).Add(item);
        }

        public void Clear() {
            ((ICollection<MinecraftVersionListing>) Versions).Clear();
        }

        public bool Contains(MinecraftVersionListing item) {
            return ((ICollection<MinecraftVersionListing>) Versions).Contains(item);
        }

        public void CopyTo(MinecraftVersionListing[] array, int arrayIndex) {
            ((ICollection<MinecraftVersionListing>) Versions).CopyTo(array, arrayIndex);
        }

        public IEnumerator<MinecraftVersionListing> GetEnumerator() {
            return ((ICollection<MinecraftVersionListing>) Versions).GetEnumerator();
        }

        public bool Remove(MinecraftVersionListing item) {
            return ((ICollection<MinecraftVersionListing>) Versions).Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((ICollection<MinecraftVersionListing>) Versions).GetEnumerator();
        }
    }
}
