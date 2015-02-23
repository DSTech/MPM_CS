using MPM.Core;
using NServiceKit.ServiceHost;

namespace MPM.Net.DTO {
	[Route("/{PackageName}", Verbs = "GET")]
	public class PackageRequest : IReturn<Package> {
		public string PackageName { get; set; }
	}
}
