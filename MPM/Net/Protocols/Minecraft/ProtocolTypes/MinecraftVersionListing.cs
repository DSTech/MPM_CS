using System;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    public class MinecraftVersionListing {
        public MinecraftVersionListing() {
        }

        public MinecraftVersionListing(
            string id,
            DateTime releaseTime,
            ReleaseType releaseType
            ) {
            this.Id = id;
            this.ReleaseTime = releaseTime;
            this.Type = releaseType;
        }

        public string Id { get; set; }

        /// <summary>
        ///     Time of release in string format, eg: 2014-05-14T19:29:23+02:00
        /// </summary>
        public DateTime ReleaseTime { get; set; }

        /// <summary>
        ///     <see cref="ReleaseType" /> of version, eg <see cref="ReleaseType.Release" /> or <see cref="ReleaseType.Snapshot" />
        /// </summary>
        public ReleaseType Type { get; set; }
    }
}
