using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPM;
using MPM.Core.Archival;
using MPM.Core.Dependency;
using System.Linq;
using MPM.Net.DTO;
using semver.tools;
using System.Collections.Generic;

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
					Version = SemanticVersion.Parse("0.0.1"),
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
				},
			};
			var testPackageSpec = new PackageSpec {
				Name = "testPackage",
				Version = new VersionSpec(SemanticVersion.Parse("0.0.2")),
				Manual = true,
			};
			var testConfig = new Configuration {
				Packages = new PackageSpec[] {
					testPackageSpec,
				},
			};
			var lookupPackageSpec = new PackageSpecLookup(packageSpec => {
				if (packageSpec.Name != "testPackage") {
					throw new ArgumentOutOfRangeException(nameof(packageSpec), "Packages that are not in- or dependencies of- the request input should not be looked up by the resolver");
				}
				return new Package {
					Authors = new[] { "testAuthor" },
					Name = "testPackage",
					Builds = testPackageBuilds,
				}.ToNamedBuilds().Where(b => packageSpec.Version.Satisfies(b.Version)).ToArray();
			});
			var resultant = resolver.Resolve(testConfig, lookupPackageSpec);
			Assert.IsTrue(
				testConfig
					.Packages
					.Where(p => p.Manual)
					.Where(p => !resultant.Packages.Any(nb => nb.Name == p.Name))
					.Count() == 0,
				"All manual packages must be accounted for in the resulting configuration"
			);
			Assert.IsTrue(
				resultant
					.Packages
					.Where(p => !testConfig.Packages.Any(nb => nb.Name == p.Name))
					.Count() == 0,
				"The resolver may not add manual packages to the resulting configuration"
			);
			Assert.IsTrue(resultant.Packages.Any(build => build.Name == testPackageSpec.Name));
		}
		[TestMethod]
		public void ResolutionWithDependencies() {
			var resolver = new Resolver();
			var dependentPackageBuilds = new[] {
				new Build {
					Conflicts = new PackageConflict[0],
					Dependencies = new [] {
						new PackageDependency {
							Name = "anticedentPackage",
							Version = new VersionSpec(SemanticVersion.Parse("0.0.3")),
						},
					},
					Hashes = new string[0],
					GivenVersion = "0.0.1.218",
					InterfaceProvisions = new InterfaceProvision[0],
					InterfaceRequirements = new InterfaceDependency[0],
					Stable = true,
					Version = semver.tools.SemanticVersion.Parse("0.0.1"),
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
				},
			};
			var anticedentPackageBuilds = new[] {
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
				},
			};
			var dependentPackageSpec = new PackageSpec {
				Name = "dependentPackage",
				Version = new VersionSpec(SemanticVersion.Parse("0.0.0"), false, SemanticVersion.Parse("0.0.2"), true),
				Manual = true,
			};
			var dependentConfig = new Configuration {
				Packages = new PackageSpec[] {
					dependentPackageSpec,
				},
			};
			var lookupPackageSpec = new PackageSpecLookup(packageSpec => {
				if (packageSpec.Name == "dependentPackage") {
					return new Package {
						Authors = new[] { "dependentAuthor" },
						Name = packageSpec.Name,
						Builds = dependentPackageBuilds,
					}.ToNamedBuilds().Where(b => packageSpec.Version.Satisfies(b.Version)).ToArray();
				} else if (packageSpec.Name == "anticedentPackage") {
					return new Package {
						Authors = new[] { "anticedentAuthor" },
						Name = packageSpec.Name,
						Builds = anticedentPackageBuilds,
					}.ToNamedBuilds().Where(b => packageSpec.Version.Satisfies(b.Version)).ToArray();
				} else {
					throw new ArgumentOutOfRangeException(nameof(packageSpec), "Packages that are not in- or dependencies of- the request input should not be looked up by the resolver");
				}
			});
			var resultant = resolver.Resolve(dependentConfig, lookupPackageSpec);
			Assert.IsTrue(
				dependentConfig
					.Packages
					.Where(p => p.Manual)
					.Where(p => !resultant.Packages.Any(nb => nb.Name == p.Name))
					.Count() == 0,
				"All manual packages must be accounted for in the resulting configuration"
			);
		}

		[TestMethod]
		public void SortBuilds() {
			var r = new Resolver();
			var builds = new NamedBuild[] {
				new NamedBuild {
					Name = "A",
					Dependencies = new [] {
						new PackageDependency {
							Name = "B"
						}
					},
				},
				new NamedBuild {
					Name = "B",
					Dependencies = new [] {
						new PackageDependency {
							Name = "A"
						}
					},
				},
				new NamedBuild {
					Name = "C",
					Dependencies = new PackageDependency[0],
				},
			};
			//Input must not contain self-dependency
			Assert.IsFalse(builds.Any(build => build.Dependencies.Any(dependency => dependency.Name == build.Name)), "The input must not contain self-dependent packages");
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
				Assert.IsTrue(
					wasCircular,
					"At least one input must be directly circularly-dependent to properly test the functionality"
				);
			}
			//Input must fulfill all dependencies
			{
				var packageNames = new SortedSet<string>(builds.Select(b => b.Name));
				foreach (var build in builds) {
					foreach (var dep in build.Dependencies) {
						CollectionAssert.Contains(packageNames, dep.Name, "The input build array should contain all dependencies to allow sorting");
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
				Assert.IsFalse(sorted, "The input build array should not be initially sorted");
			}
			var output = r.SortBuilds(builds);
			//Output must be sorted
			{
				var namesSeen = new SortedSet<string>();
				foreach (var build in output) {
					namesSeen.Add(build.Name);
					foreach (var dep in build.Dependencies) {
						if (!namesSeen.Contains(dep.Name)) {
							//Only fail here if the object is not codependent with the dependency and the dependency is not previously seen
							Assert.IsTrue(
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
