using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MPM.Core.Dependency;
using MPM.Net.DTO;
using semver.tools;
using Xunit;

namespace MPMTest.Core.Dependency {
	public class ResolverTests {
		[Fact]
		public async Task Empty() {
			Assert.True((await new Resolver().Resolve(Configuration.Empty, new TestPackageRepository(new Package[0]))).Packages.Length == 0);
		}
		[Fact]
		public async Task ResolutionNoDependencies() {
			var resolver = new Resolver();
			var testPackageSpec = new PackageSpec {
				Name = "testPackage",
				Version = new VersionSpec(SemanticVersion.Parse("0.0.2")),
				Manual = true,
				Arch = "testArch",
				Platform = "testPlatform",
			};
			var testConfig = new Configuration {
				Packages = new PackageSpec[] {
					testPackageSpec,
				},
			};
			var testRepository = new TestPackageRepository(new[] {
				new Package {
					Authors = new[] { "testAuthor" },
					Name = "testPackage",
					Builds = new[] {
						new Build {
							Conflicts = new PackageConflict[0],
							Dependencies = new PackageDependency[0],
							Hashes = new string[0],
							GivenVersion = "0.0.1.218",
							InterfaceProvisions = new InterfaceProvision[0],
							InterfaceRequirements = new InterfaceDependency[0],
							Stable = true,
							Version = SemanticVersion.Parse("0.0.1"),
							Arch = "testArch",
							Platform = "testPlatform",
						},
						new Build {
							Conflicts = new PackageConflict[0],
							Dependencies = new PackageDependency[0],
							Hashes = new string[0],
							GivenVersion = "0.0.2.219",
							InterfaceProvisions = new InterfaceProvision[0],
							InterfaceRequirements = new InterfaceDependency[0],
							Stable = true,
							Version = SemanticVersion.Parse("0.0.2"),
							Arch = "testArch",
							Platform = "testPlatform",
						},
					},
				},
			});
			var resultant = await resolver.Resolve(testConfig, testRepository);
			Assert.True(
				testConfig
					.Packages
					.Where(p => p.Manual)
					.Where(p => !resultant.Packages.Any(nb => nb.Name == p.Name))
					.Count() == 0,
				"All manual packages must be accounted for in the resulting configuration"
			);
			Assert.True(
				resultant
					.Packages
					.Where(p => !testConfig.Packages.Any(nb => nb.Name == p.Name))
					.Count() == 0,
				"The resolver may not add manual packages to the resulting configuration"
			);
			Assert.True(resultant.Packages.Any(build => build.Name == testPackageSpec.Name));
		}
		[Fact]
		public async Task ResolutionWithDependencies() {
			var resolver = new Resolver();
			var dependentPackageSpec = new PackageSpec {
				Name = "dependentPackage",
				Version = new VersionSpec(SemanticVersion.Parse("0.0.0"), false, SemanticVersion.Parse("0.0.2"), true),
				Manual = true,
				Arch = "testArch",
				Platform = "testPlatform",
			};
			var dependentConfig = new Configuration {
				Packages = new PackageSpec[] {
					dependentPackageSpec,
				},
			};
			var lookupPackageSpec = new TestPackageRepository(new[] {
				new Package {
					Name = "dependentPackage",
					Authors = new[] { "dependentAuthor" },
					Builds = new[] {
						new Build {
							Conflicts = new PackageConflict[0],
							Dependencies = new [] {
								new PackageDependency {
									Name = "anticedentPackage",
									Version = new VersionSpec(SemanticVersion.Parse("0.0.4")),
								},
							},
							Hashes = new string[0],
							GivenVersion = "0.0.1.218",
							InterfaceProvisions = new InterfaceProvision[0],
							InterfaceRequirements = new InterfaceDependency[0],
							Stable = true,
							Version = semver.tools.SemanticVersion.Parse("0.0.1"),
							Side = PackageSide.Universal,
							Arch = "testArch",
							Platform = "testPlatform",
						},
						new Build {
							Conflicts = new PackageConflict[0],
							Dependencies = new [] {
								new PackageDependency {
									Name = "anticedentPackage",
									Version = new VersionSpec(SemanticVersion.Parse("0.0.9")),
								},
							},
							Hashes = new string[0],
							GivenVersion = "0.0.2.219",
							InterfaceProvisions = new InterfaceProvision[0],
							InterfaceRequirements = new [] {
								new InterfaceDependency { Name = "anticedentInterface" },
							},
							Stable = true,
							Version = semver.tools.SemanticVersion.Parse("0.0.2"),
							Side = PackageSide.Universal,
							Arch = "testArch",
							Platform = "testPlatform",
						},
					},
				},
				new Package {
					Name = "anticedentPackage",
					Authors = new[] { "anticedentAuthor" },
					Builds = new[] {
						new Build {
							Conflicts = new PackageConflict[0],
							Dependencies = new PackageDependency[0],
							Hashes = new string[0],
							GivenVersion = "1.0RC3",
							InterfaceProvisions = new[] {
								new InterfaceProvision { Name = "anticedentInterface" },
							},
							InterfaceRequirements = new InterfaceDependency[0],
							Stable = true,
							Version = SemanticVersion.Parse("0.0.4"),
							Side = PackageSide.Universal,
							Arch = "testArch",
							Platform = "testPlatform",
						},
					},
				}
			});
			var resultant = await resolver.Resolve(dependentConfig, lookupPackageSpec);
			Assert.True(resultant.Packages.Length == 2);
			Assert.True(resultant.Packages.First().Name == "anticedentPackage", "Dependencies should appear before their dependent children");
			Assert.True(resultant.Packages.Last().Name == "dependentPackage", "Dependent packages should occur after their dependencies");
			Assert.True(
				dependentConfig
					.Packages
					.Where(p => p.Manual)
					.Where(p => !resultant.Packages.Any(nb => nb.Name == p.Name))
					.Count() == 0,
				"All manual packages must be accounted for in the resulting configuration"
			);
		}
		[Fact]
		public async Task ResolutionSideDependencies() {
			var resolver = new Resolver();
			var dependentPackageBuilds = new[] {
				new Build {
					Conflicts = new PackageConflict[0],
					Dependencies = new [] {
						new PackageDependency {
							Name = "anticedentPackage",
							Version = new VersionSpec(SemanticVersion.Parse("1.0.0"), true, SemanticVersion.Parse("1.0.3"), true),
						},
					},
					Hashes = new string[0],
					GivenVersion = "0.0.1.218",
					InterfaceProvisions = new InterfaceProvision[0],
					InterfaceRequirements = new InterfaceDependency[0],
					Stable = true,
					Version = semver.tools.SemanticVersion.Parse("0.0.1"),
					Side = PackageSide.Universal,
					Arch = "testArch",
					Platform = "testPlatform",
				},
				new Build {
					Conflicts = new PackageConflict[0],
					Dependencies = new [] {
						new PackageDependency {
							Name = "anticedentPackage",
							Version = new VersionSpec(SemanticVersion.Parse("1.0.0"), true, SemanticVersion.Parse("1.0.4"), true),
						},
					},
					Hashes = new string[0],
					GivenVersion = "0.0.2.219",
					InterfaceProvisions = new InterfaceProvision[0],
					InterfaceRequirements = new InterfaceDependency[0],
					Stable = true,
					Version = semver.tools.SemanticVersion.Parse("0.0.2"),
					Side = PackageSide.Universal,
					Arch = "testArch",
					Platform = "testPlatform",
				},
			};
			var anticedentPackageBuilds = new[] {
				new Build {
					Conflicts = new PackageConflict[0],
					Dependencies = new PackageDependency[0],
					Hashes = new string[0],
					GivenVersion = "1.0RC2_Universal",
					InterfaceProvisions = new InterfaceProvision[0],
					InterfaceRequirements = new InterfaceDependency[0],
					Stable = true,
					Version = SemanticVersion.Parse("1.0.2"),
					Side = PackageSide.Universal,
					Arch = "testArch",
					Platform = "testPlatform",
				},
				new Build {
					Conflicts = new PackageConflict[0],
					Dependencies = new PackageDependency[0],
					Hashes = new string[0],
					GivenVersion = "1.0RC3_Client",
					InterfaceProvisions = new InterfaceProvision[0],
					InterfaceRequirements = new InterfaceDependency[0],
					Stable = true,
					Version = SemanticVersion.Parse("1.0.3"),
					Side = PackageSide.Client,
					Arch = "testArch",
					Platform = "testPlatform",
				},
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
			};
			var testRepository = new TestPackageRepository(new[] {
				new Package {
					Name = "dependentPackage",
					Authors = new[] { "dependentAuthor" },
					Builds = dependentPackageBuilds,
				},
				new Package {
					Name = "anticedentPackage",
					Authors = new[] { "anticedentAuthor" },
					Builds = anticedentPackageBuilds,
				}
			});
			var dependentConfigSameVersionSidePref = new Configuration {
				Packages = new PackageSpec[] {
					new PackageSpec {
						Name = "dependentPackage",
						Version = new VersionSpec(SemanticVersion.Parse("0.0.1")),
						Manual = true,
						Arch = "testArch",
						Platform = "testPlatform",
					},
				},
				Side = PackageSide.Client,
			};
			var resultantConfigSameVersionSidePref = await resolver.Resolve(dependentConfigSameVersionSidePref, testRepository);
			Assert.True(
				resultantConfigSameVersionSidePref
					.Packages
					.Where(p => p.Name == "anticedentPackage")
					.All(p => p.GivenVersion == "1.0RC3_Client"),
				"Packages should prefer those of the same side when versions are equal"
			);
			var dependentConfigDifferentVersionVersionPref = new Configuration {
				Packages = new PackageSpec[] {
					new PackageSpec {
						Name = "dependentPackage",
						Version = new VersionSpec(SemanticVersion.Parse("0.0.2")),
						Manual = true,
						Arch = "testArch",
						Platform = "testPlatform",
					},
				},
				Side = PackageSide.Client,
			};
			var resultantConfigDifferentVersionVersionPref = await resolver.Resolve(dependentConfigDifferentVersionVersionPref, testRepository);
			Assert.True(
				resultantConfigDifferentVersionVersionPref
					.Packages
					.Where(p => p.Name == "anticedentPackage")
					.All(p => p.GivenVersion == "1.0RC4_Universal"),
				"Packages should prefer those of the higher version, even when the preferred side are available for a lower version"
			);
		}
		[Fact]
		public void SortBuilds() {
			var r = new Resolver();
			var builds = new NamedBuild[] {
				new NamedBuild {
					Name = "A",
					Dependencies = new [] {
						new PackageDependency {
							Name = "B"
						},
						new PackageDependency {
							Name = "C"
						},
					},
				},
				new NamedBuild {
					Name = "B",
					Dependencies = new [] {
						new PackageDependency {
							Name = "A"
						},
						new PackageDependency {
							Name = "D"
						},
					},
				},
				new NamedBuild {
					Name = "C",
					Dependencies = new [] {
						new PackageDependency {
							Name = "D"
						},
					},
				},
				new NamedBuild {
					Name = "D",
					Dependencies = new PackageDependency[0],
				},
			};
			//Input must not contain self-dependency
			Assert.False(builds.Any(build => build.Dependencies.Any(dependency => dependency.Name == build.Name)), "The input must not contain self-dependent packages");
			//At least one input must be circularly-dependent
			{
				var wasCircular = false;
				foreach (var build in builds) {
					//A build must refer to something that refers back to this one
					if (build.Dependencies.Any(
						second => builds
							.Where(other => other.Name == second.Name)
							.Select(other => other.Dependencies)
							.Any(otherDeps => otherDeps.Any(dep => dep.Name == build.Name))
					)) {
						wasCircular = true;
						break;
					}
				}
				Assert.True(
					wasCircular,
					"At least one input must be directly circularly-dependent to properly test the functionality"
				);
			}
			//Input must fulfill all dependencies
			{
				var packageNames = new SortedSet<string>(builds.Select(b => b.Name));
				foreach (var build in builds) {
					foreach (var dep in build.Dependencies) {
						Assert.True(packageNames.Contains(dep.Name), "The input build array should contain all dependencies to allow sorting");
					}
				}
			}
			//Input must not sorted so the test accomplishes something
			{
				var namesSeen = new SortedSet<string>();
				bool sorted = true;
				foreach (var build in builds) {
					namesSeen.Add(build.Name);
					foreach (var dep in build.Dependencies) {
						if (!namesSeen.Contains(dep.Name)) {
							sorted = false;
							break;
						}
					}
					if (sorted == false) {
						break;
					}
				}
				Assert.False(sorted, "The input build array should not be initially sorted");
			}
			var output = r.SortBuilds(builds).ToArray();
			//Dependencies must appear in order of least dependence
			{
				Assert.True(output.Length == 4);
				var orderErrMsg = "Dependencies must occur in order of least dependence";
				Assert.True(output[0].Name == "D", orderErrMsg);
				Assert.True(output[1].Name == "B", orderErrMsg);
				Assert.True(output[2].Name == "C", orderErrMsg);
				Assert.True(output[3].Name == "A", orderErrMsg);
			}
			//Output must be sorted
			{
				var namesSeen = new SortedSet<string>();
				foreach (var build in output) {
					namesSeen.Add(build.Name);
					foreach (var dep in build.Dependencies) {
						if (!namesSeen.Contains(dep.Name)) {
							//Only fail here if the object is not codependent with the dependency and the dependency is not previously seen
							Assert.True(
								namesSeen.Contains(dep.Name) ||
								output
									.Where(other => other.Name == dep.Name)
									.Select(other => other.Dependencies)
									.Any(otherDeps => otherDeps.Any(dependency => dependency.Name == build.Name)),
								"The output build array must be sorted"
							);
						}
					}
				}
			}
		}
	}
}
