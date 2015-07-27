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

	public class MinecraftVersionCollection : ICollection<MinecraftVersion> {

		public MinecraftVersionLatestSpec Latest { get; set; } = new MinecraftVersionLatestSpec {
			Release = null,
			Snapshot = null,
		};

		public MinecraftVersion[] Versions { get; set; } = new MinecraftVersion[0];

		public int Count {
			get {
				return ((ICollection<MinecraftVersion>)Versions).Count;
			}
		}

		public bool IsReadOnly {
			get {
				return ((ICollection<MinecraftVersion>)Versions).IsReadOnly;
			}
		}

		public void Add(MinecraftVersion item) {
			((ICollection<MinecraftVersion>)Versions).Add(item);
		}

		public void Clear() {
			((ICollection<MinecraftVersion>)Versions).Clear();
		}

		public bool Contains(MinecraftVersion item) {
			return ((ICollection<MinecraftVersion>)Versions).Contains(item);
		}

		public void CopyTo(MinecraftVersion[] array, int arrayIndex) {
			((ICollection<MinecraftVersion>)Versions).CopyTo(array, arrayIndex);
		}

		public IEnumerator<MinecraftVersion> GetEnumerator() {
			return ((ICollection<MinecraftVersion>)Versions).GetEnumerator();
		}

		public bool Remove(MinecraftVersion item) {
			return ((ICollection<MinecraftVersion>)Versions).Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return ((ICollection<MinecraftVersion>)Versions).GetEnumerator();
		}
	}
}
