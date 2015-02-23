using System.Collections.Generic;
using MPM.Core;
using NServiceKit.ServiceHost;

namespace MPM.Net.DTO {
	[Route("/", Verbs = "GET")]
	[Route("/{PackageNames}", Verbs = "GET")]
	public class PackagesRequest : IReturn<IEnumerable<Package>> {
		public string[] PackageNames { get; set; }
	}
}
