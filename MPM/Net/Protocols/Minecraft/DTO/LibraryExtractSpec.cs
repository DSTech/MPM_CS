namespace MPM.Net.Protocols.Minecraft.DTO {

	public class LibraryExtractSpec {

		/// <summary>
		/// A list of paths to exclude from extraction when installing the library
		/// </summary>
		public string[] Exclude { get; set; } = new string[0];
	}
}
