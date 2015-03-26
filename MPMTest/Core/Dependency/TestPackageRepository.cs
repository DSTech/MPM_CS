using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Data;
using MPM.Net.DTO;
using semver.tools;

namespace MPMTest.Core.Dependency {
	public class TestPackageRepository : IPackageRepository {
		private Package[] package;

		public TestPackageRepository(Package[] package) {
			this.package = package;
		}

		public Task<Build> FetchBuild(string packageName, SemanticVersion version) {
			throw new NotImplementedException();
		}

		public Task<Package> FetchBuilds(string packageName, VersionSpec versionSpec) {
			throw new NotImplementedException();
		}

		public Task<Package> FetchPackage(string packageName) {
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Package>> FetchPackageList() {
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Package>> FetchPackageList(DateTime updatedAfter) {
			throw new NotImplementedException();
		}
	}
}
