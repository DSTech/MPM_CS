using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Extensions;

namespace MPM.Net.Protocols.Minecraft.DTO {

	public class MinecraftVersionListing {
		public string Id { get; set; }

		/// <summary>
		/// Time of release in string format, eg: 2014-05-14T19:29:23+02:00
		/// </summary>
		public string Time { get; set; }

		/// <summary>
		/// Alias of <see cref="Time"/>
		/// </summary>
		public string ReleaseTime { get; set; }

		/// <summary>
		/// Type of version, eg: "Release" or "Snapshot"
		/// </summary>
		public string Type { get; set; }
	}
}
