namespace MPM.Types {
	public enum CompatibilityPlatform {
		Lin32,//Linux x32
		Lin64,//Linux x64
		Lin,//Linux x32/x64

		Win32,//Windows x32
		Win64,//Windows x64
		Win,//Windows x32/x64

		Universal32,//Platform independent x32
		Universal64,//Platform independent x64
		Universal//Platform and bitness independent
	}
}
