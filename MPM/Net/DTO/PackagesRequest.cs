using System;
using System.Collections.Generic;
using MPM.Core;
using NServiceKit.ServiceHost;

namespace MPM.Net.DTO {
	[Route("/packages", Verbs = "GET")]
	[Route("/packages/{PackageNames}", Verbs = "GET")]
	public class PackagesRequest : IReturn<IEnumerable<Package>> {
		public string[] PackageNames { get; set; }
		public DateTime? LastSync { get; set; }
	}
}
