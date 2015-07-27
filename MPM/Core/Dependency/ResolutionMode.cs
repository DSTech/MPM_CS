namespace MPM.Core.Dependency {

	public enum ResolutionMode {

		/// <summary>
		/// Highest matching
		/// </summary>
		Highest,

		/// <summary>
		/// Highest matching where stable, otherwise highest if no matching stable build exists
		/// </summary>
		HighestStable,
	}
}
