using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Data;
using MPM.Net.DTO;
using semver.tools;
using MPM.Core.Dependency;
using Xunit;

namespace MPMTest.Core.Dependency {
	public class TestPackageRepositoryTests {
		[Fact]
		public static async Task TestPackageRepositoryOrdering() {
			var repo = new TestPackageRepository(new[] {
					new Package() {
					Name = "testPackage",
					Authors = new string[] { "testAuthor" },
					Builds = new[] {
						new Build {
							Conflicts = new PackageConflict[0],
							Dependencies = new PackageDependency[0],
							Hashes = new string[0],
							GivenVersion = "1.0RC3_Universal",
							InterfaceProvisions = new InterfaceProvision[0],
							InterfaceRequirements = new InterfaceDependency[0],
							Stable = true,
							Version = SemanticVersion.Parse("1.0.3"),
							Side = PackageSide.Universal,
							Arch = "testArch",
							Platform = "testPlatform",
						},
						new Build {
							Conflicts = new PackageConflict[0],
							Dependencies = new PackageDependency[0],
							Hashes = new string[0],
							GivenVersion = "1.0RC4_Universal",
							InterfaceProvisions = new InterfaceProvision[0],
							InterfaceRequirements = new InterfaceDependency[0],
							Stable = true,
							Version = SemanticVersion.Parse("1.0.4"),
							Side = PackageSide.Universal,
							Arch = "testArch",
							Platform = "testPlatform",
						},
					},
				}
			});

			var testSpec = new PackageSpec {
				Name = "testPackage",
				Version = new VersionSpec(SemanticVersion.Parse("0.0.0"), true, SemanticVersion.Parse("9.9.9"), true),
				Manual = true,
				Arch = "testArch",
				Platform = "testPlatform",
			};

			var lookupRes = await repo.LookupSpec(testSpec);
			Assert.True(lookupRes.First().Version == SemanticVersion.Parse("1.0.4"), "The highest conforming version should be returned by the repository");
		}
	}
	public class TestPackageRepository : IPackageRepository {
		private Package[] packages;

		public TestPackageRepository(Package[] packages) {
			this.packages = packages;
		}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		public async Task<Build> FetchBuild(string packageName, SemanticVersion version, PackageSide side, String arch, String platform) {
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
			return packages
				.FirstOrDefault(p => p.Name == packageName)
				?.Builds
				?.Where(b => b.Version == version && b.Arch == arch && b.Platform == platform && (b.Side == side || b.Side == PackageSide.Universal))
				?.OrderByDescending(b => b.Version)//Prefer higher versions
				?.ThenByDescending(b => b.Side != PackageSide.Universal)//Prefer side-specific
				?.FirstOrDefault();
		}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		public async Task<Package> FetchBuilds(string packageName, VersionSpec versionSpec) {
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
			var basePackage = packages.FirstOrDefault(p => p.Name == packageName);
			if (basePackage == null) {
				return null;
			}
			return new Package {
				Name = basePackage.Name,
				Authors = basePackage.Authors,
				Builds = basePackage
					.Builds
					.Where(b => versionSpec.Satisfies(b.Version))
					.OrderByDescending(b => b.Version)//Prefer higher versions
					.ThenByDescending(b => b.Side != PackageSide.Universal)//Prefer side-specific
					.ToArray(),
			};
		}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		public async Task<Package> FetchPackage(string packageName) {
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
			var basePackage = packages.FirstOrDefault(p => p.Name == packageName);
			if(basePackage == null) {
				return null;
			}
			return new Package {
				Name = basePackage.Name,
				Authors = basePackage.Authors,
				Builds = basePackage
					.Builds
					.OrderByDescending(b => b.Version)//Prefer higher versions
					.ThenByDescending(b => b.Side != PackageSide.Universal)//Prefer side-specific
					.ToArray(),
			};
		}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		public async Task<IEnumerable<Package>> FetchPackageList() {
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
			return packages;
		}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		public async Task<IEnumerable<Package>> FetchPackageList(DateTime updatedAfter) {
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
			return packages;
		}
	}
}
