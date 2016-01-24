using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Net.Protocols.Minecraft.DTO {
    public class MinecraftVersion {
        public string Id { get; set; }

        /// <summary>
        ///     Time of release in string format, eg: 2014-05-14T19:29:23+02:00
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        ///     Alias of <see cref="Time" />
        /// </summary>
        public string ReleaseTime { get; set; }

        /// <summary>
        ///     Type of version, eg: "Release" or "Snapshot"
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     A string with ${placeholders} that should be substituted before launching, using the string as command line
        ///     arguments
        /// </summary>
        public string MinecraftArguments { get; set; }

        /// <summary>
        ///     Minimum launcher spec version capable of launching the particular version
        /// </summary>
        public int MinimumLauncherVersion { get; set; }

        /// <summary>
        ///     Version for which assets should be loaded, eg: "1.7.10"
        /// </summary>
        public string Assets { get; set; }

        public LibrarySpec[] Libraries { get; set; }
    }
}
