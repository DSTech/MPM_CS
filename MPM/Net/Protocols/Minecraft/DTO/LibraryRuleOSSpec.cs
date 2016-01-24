namespace MPM.Net.Protocols.Minecraft.DTO {
    public class LibraryRuleOSSpec {
        /// <summary>
        ///     The name of the OS to which the rule applies, eg: "windows", "linux", or "osx"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The version of the OS to which the rule applies
        /// </summary>
        public string Version { get; set; }
    }
}
