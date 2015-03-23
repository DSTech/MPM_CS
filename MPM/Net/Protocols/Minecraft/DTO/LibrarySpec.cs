namespace MPM.Net.Protocols.Minecraft.DTO {
	public class LibrarySpec {
		/// <summary>
		/// Name of the library being specified, eg: "com.google.code.gson:gson:2.2.4"
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// A description of native libraries to be used to supplement a particular library
		/// </summary>
		public LibraryNativesSpec Natives { get; set; }
		/// <summary>
		/// A description of how a package should be extracted, with optional exclusions
		/// </summary>
		public LibraryExtractSpec Extract { get; set; }
		/// <summary>
		/// A series of rules for whether or not a library is allowed on an operating system. Later entries take precedence over previous ones
		/// </summary>
		public LibraryRuleSpec[] Rules { get; set; } = new LibraryRuleSpec[0];
	}
}
