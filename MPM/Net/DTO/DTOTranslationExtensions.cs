using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Net.DTO {

	public static class DTOTranslationExtensions {

		private static IEnumerable<T> Denull<T>(this IEnumerable<T> enumerable) => (enumerable != null ? enumerable : Enumerable.Empty<T>());

		//CompatibilityPlatform
		public static Types.CompatibilityPlatform FromCompatibilityPlatformDTO(string platformDto) => (Types.CompatibilityPlatform)Enum.Parse(typeof(Types.CompatibilityPlatform), platformDto, true);

		public static string ToDTO(this Types.CompatibilityPlatform platform) => platform.ToString();

		//CompatibilitySide
		public static Types.CompatibilitySide FromDTO(this PackageSide side) {
			switch (side) {
				case PackageSide.Client: return Types.CompatibilitySide.Client;
				case PackageSide.Server: return Types.CompatibilitySide.Server;
				case PackageSide.Universal: return Types.CompatibilitySide.Universal;
				default: throw new NotSupportedException();
			}
		}

		public static PackageSide ToDTO(this Types.CompatibilitySide side) {
			switch (side) {
				case Types.CompatibilitySide.Client: return PackageSide.Client;
				case Types.CompatibilitySide.Server: return PackageSide.Server;
				case Types.CompatibilitySide.Universal: return PackageSide.Universal;
				default: throw new NotSupportedException();
			}
		}

		//Arch
		public static Types.Arch FromArchDTO(string arch) => new Types.Arch(arch);

		public static string ToDTO(this Types.Arch arch) => arch.Id;

		//Author
		public static Types.Author FromDTO(this Author author) => new Types.Author(author.Name, author.Email);

		public static Author ToDTO(this Types.Author author) => new Author { Name = author.Name, Email = author.Email };

		//Version
		public static semver.tools.SemanticVersion FromDTO(this Version version) => new semver.tools.SemanticVersion(version.Major, version.Minor, version.Patch);

		public static Version ToDTO(this semver.tools.SemanticVersion version) => Version.Parse(version.ToString());

		//VersionSpec
		public static semver.tools.VersionSpec FromDTO(this VersionSpec version) => new semver.tools.VersionSpec(
			version.Minimum.FromDTO(),
			version.MinInclusive,
			version.Maximum.FromDTO(),
			version.MaxInclusive
		);

		public static VersionSpec ToDTO(this semver.tools.VersionSpec version) => new VersionSpec {
			Minimum = version.MinVersion.ToDTO(),
			MinInclusive = version.IsMinInclusive,
			Maximum = version.MaxVersion.ToDTO(),
			MaxInclusive = version.IsMaxInclusive,
		};

		//Package
		public static Types.Package FromDTO(this Package package) => new Types.Package(
			package.Name,
			package.Authors.Denull().Select(author => author.FromDTO()),
			package.Builds.Denull().Select(build => build.FromDTO())
		);

		public static Package ToDTO(this Types.Package package) => new Package {
			Name = package.Name,
			Authors = package.Authors.Select(author => author.ToDTO()).ToArray(),
			Builds = package.Builds.Select(build => build.ToDTO()).ToArray(),
		};

		//Build
		public static Types.Build FromDTO(this Build build) => new Types.Build(
			build.Package,
			build.Version.FromDTO(),
			build.GivenVersion,
			FromArchDTO(build.Arch),
			FromCompatibilityPlatformDTO(build.Platform),
			FromDTO(build.Side),
			build.Interfaces.Denull().Select(interfaceProvision => interfaceProvision.FromDTO()),
			build.Dependencies?.Interfaces.Denull().Select(interfaceDependency => interfaceDependency.FromDTO()),
			build.Dependencies?.Packages.Denull().Select(packageDependency => packageDependency.FromDTO()),
			build.Conflicts.Denull().Select(packageConflict => packageConflict.FromDTO()),
			build.Hashes.Denull().Select(hash => Types.Hash.Parse(hash)),
			build.Stable,
			build.Recommended
		);

		public static Build ToDTO(this Types.Build build) => new Build {
			Package = build.PackageName,
			Arch = build.Arch.ToDTO(),
			Conflicts = build.Conflicts.Select(conflict => conflict.ToDTO()).ToArray(),
			Interfaces = build.InterfaceProvisions.Select(interfaceProvision => interfaceProvision.ToDTO()).ToArray(),
			Dependencies = new DependencySpec {
				Interfaces = build.InterfaceDependencies.Select(interfaceDependency => interfaceDependency.ToDTO()).ToArray(),
				Packages = build.PackageDependencies.Select(packageDependency => packageDependency.ToDTO()).ToArray(),
			},
			GivenVersion = build.GivenVersion,
			Hashes = build.Hashes.Select(hash => hash.ToString()).ToArray(),
			Platform = build.Platform.ToDTO(),
			Recommended = build.Recommended,
			Side = build.Side.ToDTO(),
			Stable = build.Stable,
			Version = build.Version.ToDTO(),
		};

		//PackageConflict
		public static Types.Conflict FromDTO(this PackageConflict conflict) => new Types.Conflict(conflict.Condition.FromDTO(), conflict.Resolution.FromDTO());

		public static PackageConflict ToDTO(this Types.Conflict conflict) => new PackageConflict { Condition = conflict.Condition.ToDTO(), Resolution = conflict.Resolution.ToDTO() };

		//ConflictCondition
		public static Types.ConflictCondition FromDTO(this ConflictCondition conflictCondition) => new Types.ConflictCondition(
			conflictCondition.Package,
			conflictCondition.Interface,
			conflictCondition.And.Select(cond => cond.FromDTO()),
			conflictCondition.Or.Select(cond => cond.FromDTO())
		);

		public static ConflictCondition ToDTO(this Types.ConflictCondition conflictCondition) => new ConflictCondition {
			And = conflictCondition.And.Select(cond => cond.ToDTO()).ToArray(),
			Or = conflictCondition.Or.Select(cond => cond.ToDTO()).ToArray(),
			Interface = conflictCondition.InterfaceName,
			Package = conflictCondition.PackageName,
		};

		//ConflictResolution
		public static Types.ConflictResolution FromDTO(this ConflictResolution res) => new Types.ConflictResolution(res.Dependencies.FromDTO(), res.Install.FromDTO());

		public static ConflictResolution ToDTO(this Types.ConflictResolution res) => new ConflictResolution { Dependencies = res.Dependency.ToDTO(), Install = res.Installation.ToDTO() };

		//DependencyConflictResolution
		public static Types.DependencyConflictResolution FromDTO(this DependencyConflictResolution res) => new Types.DependencyConflictResolution(res.Force.FromDTO(), res.Decline.FromDTO());

		public static DependencyConflictResolution ToDTO(this Types.DependencyConflictResolution res) => new DependencyConflictResolution { Decline = res.Decline.ToDTO(), Force = res.Force.ToDTO() };

		//InstallationConflictResolution
		public static Types.InstallationConflictResolution FromDTO(this InstallationConflictResolution res) => new Types.InstallationConflictResolution(res.Packages);

		public static InstallationConflictResolution ToDTO(this Types.InstallationConflictResolution res) => new InstallationConflictResolution { Packages = res.PackageNames.ToArray() };

		//DeclinedDependencySet
		public static Types.DeclinedDependencySet FromDTO(this DeclinedDependency res) => new Types.DeclinedDependencySet(res.Packages, res.Interfaces);

		public static DeclinedDependency ToDTO(this Types.DeclinedDependencySet res) => new DeclinedDependency { Interfaces = res.InterfaceNames.ToArray(), Packages = res.PackageNames.ToArray() };

		//ForcedDependencySet
		public static Types.ForcedDependencySet FromDTO(this ForcedDependency res) => new Types.ForcedDependencySet(res.Packages, res.Interfaces);

		public static ForcedDependency ToDTO(this Types.ForcedDependencySet res) => new ForcedDependency { Interfaces = res.InterfaceNames.ToArray(), Packages = res.PackageNames.ToArray() };

		//InterfaceProvision
		public static Types.InterfaceProvision FromDTO(this InterfaceProvision prov) => new Types.InterfaceProvision(prov.Name, prov.Version.FromDTO());

		public static InterfaceProvision ToDTO(this Types.InterfaceProvision prov) => new InterfaceProvision { Name = prov.InterfaceName, Version = prov.Version.ToDTO() };

		//InterfaceDependency
		public static Types.InterfaceDependency FromDTO(this InterfaceDependency dep) => new Types.InterfaceDependency(dep.Name, dep.Version.FromDTO(), Types.CompatibilitySide.Universal);

		public static InterfaceDependency ToDTO(this Types.InterfaceDependency dep) => new InterfaceDependency { Name = dep.InterfaceName, Version = dep.VersionSpec.ToDTO() };

		//PackageDependency
		public static Types.PackageDependency FromDTO(this PackageDependency dep) => new Types.PackageDependency(dep.Name, dep.Version.FromDTO(), dep.Side.FromDTO());

		public static PackageDependency ToDTO(this Types.PackageDependency dep) => new PackageDependency { Name = dep.PackageName, Version = dep.VersionSpec.ToDTO(), Side = dep.Side.ToDTO() };
	}
}
