using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Extensions;

namespace MPM.Net.Protocols.Minecraft.Types {

	public class MinecraftVersionListing {
		public MinecraftVersionListing(
			string id,
			DateTime releaseTime,
			ReleaseType releaseType
		) {
			this.Id = id;
			this.ReleaseTime = releaseTime;
			this.Type = releaseType;
		}

		public string Id { get; }

		/// <summary>
		/// Time of release in string format, eg: 2014-05-14T19:29:23+02:00
		/// </summary>
		public DateTime ReleaseTime { get; }

		/// <summary>
		/// <see cref="ReleaseType"/> of version, eg <see cref="ReleaseType.Release"/> or <see cref="ReleaseType.Snapshot"/>
		/// </summary>
		public ReleaseType Type { get; }
	}
}
