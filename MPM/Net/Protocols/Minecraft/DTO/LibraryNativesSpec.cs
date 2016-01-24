namespace MPM.Net.Protocols.Minecraft.DTO {
    /// <summary>
    ///     Contains native-library inclusions for each particular operating system, with an optional ${arch} placeholder in
    ///     circumstances where bitness matters
    /// </summary>
    /// <example>
    ///     {
    ///     "linux": "natives-linux",
    ///     "windows": "natives-windows-${arch}",
    ///     "osx": "natives-osx",
    ///     }
    /// </example>
    public class LibraryNativesSpec {
        /// <summary>
        ///     See info for <seealso cref="LibraryNativesSpec" />
        /// </summary>
        public string Windows { get; set; }

        /// <summary>
        ///     See info for <seealso cref="LibraryNativesSpec" />
        /// </summary>
        public string Linux { get; set; }

        /// <summary>
        ///     See info for <seealso cref="LibraryNativesSpec" />
        /// </summary>
        public string Osx { get; set; }
    }
}
