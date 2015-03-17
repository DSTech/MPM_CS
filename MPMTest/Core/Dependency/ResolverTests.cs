using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPM;
using MPM.Core.Archival;
using MPM.Core.Dependency;
using System.Linq;
using MPM.Net.DTO;

namespace MPMTest {
	[TestClass]
	public class ResolverTests {
		[TestMethod]
		public void Empty() {
			Assert.IsTrue(new Resolver().Resolve(Configuration.Empty, (spec) => null).Packages.Length == 0);
		}
		[TestMethod]
		public void ResolutionNoDependencies() {
			var resolver = new Resolver();
			var testPackageBuilds = new[] {
				new Build {
					Conflicts = new PackageConflict[0],
					Dependencies = new PackageDependency[0],
					Hashes = new string[0],
					GivenVersion = "0.0.1.218",
					InterfaceProvisions = new InterfaceProvision[0],
					InterfaceRequirements = new InterfaceDependency[0],
					Stable = true,
					Version = semver.tools.SemanticVersion.Parse("0.0.1"),
				},
				new Build {
					Conflicts = new PackageConflict[0],
					Dependencies = new PackageDependency[0],
					Hashes = new string[0],
					GivenVersion = "0.0.2.219",
					InterfaceProvisions = new InterfaceProvision[0],
					InterfaceRequirements = new InterfaceDependency[0],
					Stable = true,
					Version = semver.tools.SemanticVersion.Parse("0.0.2"),
				},
			};
			var testPackageSpec = new PackageSpec {
				Name = "testPackage",
				Version = semver.tools.SemanticVersion.Parse("0.0.2"),
				Manual = true,
			};
			var testConfig = new Configuration {
				Packages = new PackageSpec[] {
					testPackageSpec,
				},
			};
			var lookupBuild = new Func<PackageSpec, Package>(packageSpec => {
				if (packageSpec.Name != "testPackage") {
					throw new ArgumentOutOfRangeException(nameof(packageSpec), "Packages that are not in- or dependencies of- the request input should not be looked up by the resolver");
				}
				return new Package {
					Authors = new[] { "testAuthor" },
					Name = "testPackage",
					Builds = new[] {
						testPackageBuilds.First(b => b.Version == packageSpec.Version),
					},
				};
			});
			var resultant = resolver.Resolve(testConfig, lookupBuild);
			Assert.IsTrue(
				testConfig
					.Packages
					.Where(p => p.Manual)
					.Except(resultant.Packages)
					.Count() == 0,
				"All manual packages must be accounted for in the resulting configuration"
			);
			Assert.IsTrue(
				resultant
					.Packages
					.Where(p => p.Manual)
					.Except(testConfig.Packages)
					.Count() == 0,
				"The resolver may not add manual packages to the resulting configuration"
			);
			CollectionAssert.Contains(resultant.Packages, testPackageSpec);
		}
		[TestMethod]
		public void ResolutionWithDependencies() {
			var resolver = new Resolver();
			var dependentPackageBuilds = new[] {
				new Build {
					Conflicts = new PackageConflict[0],
					Dependencies = new PackageDependency[0],
					Hashes = new string[0],
					GivenVersion = "0.0.1.218",
					InterfaceProvisions = new InterfaceProvision[0],
					InterfaceRequirements = new InterfaceDependency[0],
					Stable = true,
					Version = semver.tools.SemanticVersion.Parse("0.0.1"),
				},
				new Build {
					Conflicts = new PackageConflict[0],
					Dependencies = new PackageDependency[0],
					Hashes = new string[0],
					GivenVersion = "0.0.2.219",
					InterfaceProvisions = new InterfaceProvision[0],
					InterfaceRequirements = new [] {
						new InterfaceDependency { Name = "anticedent" },
					},
					Stable = true,
					Version = semver.tools.SemanticVersion.Parse("0.0.2"),
				},
			};
			var anticedentPackageBuilds = new[] {
				new Build {
					Conflicts = new PackageConflict[0],
					Dependencies = new PackageDependency[0],
					Hashes = new string[0],
					GivenVersion = "1.0RC3",
					InterfaceProvisions = new[] {
						new InterfaceProvision { Name = "anticedent" },
					},
					InterfaceRequirements = new InterfaceDependency[0],
					Stable = true,
					Version = semver.tools.SemanticVersion.Parse("0.0.4"),
				},
			};
			var dependentPackageSpec = new PackageSpec {
				Name = "testPackage",
				Version = semver.tools.SemanticVersion.Parse("0.0.2"),
				Manual = true,
			};
			var dependentConfig = new Configuration {
				Packages = new PackageSpec[] {
					dependentPackageSpec,
				},
			};
			var lookupBuild = new Func<PackageSpec, Package>(packageSpec => {
				if (packageSpec.Name == "dependentPackage") {
					return new Package {
						Authors = new[] { "dependentAuthor" },
						Name = packageSpec.Name,
						Builds = new[] {
						dependentPackageBuilds.First(b => b.Version == packageSpec.Version),
					},
					};
				} else if (packageSpec.Name == "anticedentPackage") {
					return new Package {
						Authors = new[] { "anticedentAuthor" },
						Name = packageSpec.Name,
						Builds = new[] {
						anticedentPackageBuilds.First(b => b.Version == packageSpec.Version),
					},
					};
				} else {
					throw new ArgumentOutOfRangeException(nameof(packageSpec), "Packages that are not in- or dependencies of- the request input should not be looked up by the resolver");
				}
			});
			var resultant = resolver.Resolve(dependentConfig, lookupBuild);
			Assert.IsTrue(
				dependentConfig
					.Packages
					.Where(p => p.Manual)
					.Except(resultant.Packages)
					.Count() == 0,
				"All manual packages must be accounted for in the resulting configuration"
			);
			Assert.IsTrue(
				resultant
					.Packages
					.Where(p => p.Manual)
					.Except(dependentConfig.Packages)
					.Count() == 0,
				"The resolver may not add manual packages to the resulting configuration"
			);
		}
	}
}
