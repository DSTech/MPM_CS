using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Extensions;

namespace MPM.Net {

	public static class DTOTranslationExtensions {

		//CompatibilityPlatform
		public static Types.CompatibilityPlatform FromCompatibilityPlatformDTO(string platformDto) => (Types.CompatibilityPlatform)System.Enum.Parse(typeof(Types.CompatibilityPlatform), platformDto, true);

		public static string ToDTO(this Types.CompatibilityPlatform platform) => platform.ToString();

		//CompatibilitySide
		public static Types.CompatibilitySide FromDTO(this DTO.PackageSide side) {
			switch (side) {
				case DTO.PackageSide.Client: return Types.CompatibilitySide.Client;
				case DTO.PackageSide.Server: return Types.CompatibilitySide.Server;
				case DTO.PackageSide.Universal: return Types.CompatibilitySide.Universal;
				default: throw new System.NotSupportedException();
			}
		}

		public static DTO.PackageSide ToDTO(this Types.CompatibilitySide side) {
			switch (side) {
				case Types.CompatibilitySide.Client: return DTO.PackageSide.Client;
				case Types.CompatibilitySide.Server: return DTO.PackageSide.Server;
				case Types.CompatibilitySide.Universal: return DTO.PackageSide.Universal;
				default: throw new System.NotSupportedException();
			}
		}

		//Arch
		public static Types.Arch FromArchDTO(string arch) => new Types.Arch(arch);

		public static string ToDTO(this Types.Arch arch) => arch.Id;

		//Author
		public static Types.Author FromDTO(this DTO.Author author) => new Types.Author(author.Name, author.Email);

		public static DTO.Author ToDTO(this Types.Author author) => new DTO.Author { Name = author.Name, Email = author.Email };

		//Version
		public static semver.tools.SemanticVersion FromDTO(this DTO.Version version) => new semver.tools.SemanticVersion(version.Major, version.Minor, version.Patch);

		public static DTO.Version ToDTO(this semver.tools.SemanticVersion version) => DTO.Version.Parse(version.ToString());

		//VersionSpec
		public static semver.tools.VersionSpec FromDTO(this DTO.VersionSpec version) => new semver.tools.VersionSpec(
			version.Minimum.FromDTO(),
			version.MinInclusive,
			version.Maximum.FromDTO(),
			version.MaxInclusive
		);

		public static DTO.VersionSpec ToDTO(this semver.tools.VersionSpec version) => new DTO.VersionSpec {
			Minimum = version.MinVersion.ToDTO(),
			MinInclusive = version.IsMinInclusive,
			Maximum = version.MaxVersion.ToDTO(),
			MaxInclusive = version.IsMaxInclusive,
		};

		//Package
		public static Types.Package FromDTO(this DTO.Package package) => new Types.Package(
			package.Name,
			package.Authors.Denull().Select(author => author.FromDTO()),
			package.Builds.Denull().Select(b => {
				if (b.Package == null) {
					b.Package = package.Name;
				}
				return b;
			}).Select(build => build.FromDTO())
		);

		public static DTO.Package ToDTO(this Types.Package package) => new DTO.Package {
			Name = package.Name,
			Authors = package.Authors.Select(author => author.ToDTO()).ToArray(),
			Builds = package.Builds.Select(build => build.ToDTO()).ToArray(),
		};

		//Build
		public static Types.Build FromDTO(this DTO.Build build) => new Types.Build(
			build.Package,
			build.Version.FromDTO(),
			build.GivenVersion,
			FromArchDTO(build.Arch),
			build.Platform != null ? FromCompatibilityPlatformDTO(build.Platform) : Types.CompatibilityPlatform.Universal,
			FromDTO(build.Side),
			build.Interfaces.Denull().Select(interfaceProvision => interfaceProvision.FromDTO()),
			build.Dependencies?.Interfaces.Denull().Select(interfaceDependency => interfaceDependency.FromDTO()),
			build.Dependencies?.Packages.Denull().Select(packageDependency => packageDependency.FromDTO()),
			build.Conflicts.Denull().Select(packageConflict => packageConflict.FromDTO()),
			build.Hashes.Denull().Select(hash => Types.Hash.Parse(hash)),
			build.Stable,
			build.Recommended
		);

		public static DTO.Build ToDTO(this Types.Build build) => new DTO.Build {
			Package = build.PackageName,
			Arch = build.Arch.ToDTO(),
			Conflicts = build.Conflicts.Select(conflict => conflict.ToDTO()).ToArray(),
			Interfaces = build.InterfaceProvisions.Select(interfaceProvision => interfaceProvision.ToDTO()).ToArray(),
			Dependencies = new DTO.DependencySpec {
				Interfaces = build.InterfaceDependencies.Select(interfaceDependency => interfaceDependency.ToDTO()).ToArray(),
				Packages = build.PackageDependencies.Select(packageDependency => packageDependency.ToDTO()).ToArray(),
			},
			GivenVersion = build.GivenVersion,
			Hashes = build.Hashes.Denull().Select(hash => hash.ToString()).ToArray(),
			Platform = build.Platform.ToDTO(),
			Recommended = build.Recommended,
			Side = build.Side.ToDTO(),
			Stable = build.Stable,
			Version = build.Version.ToDTO(),
		};

		//PackageConflict
		public static Types.Conflict FromDTO(this DTO.PackageConflict conflict) => new Types.Conflict(conflict.Condition.FromDTO(), conflict.Resolution.FromDTO());

		public static DTO.PackageConflict ToDTO(this Types.Conflict conflict) => new DTO.PackageConflict { Condition = conflict.Condition.ToDTO(), Resolution = conflict.Resolution.ToDTO() };

		//ConflictCondition
		public static Types.ConflictCondition FromDTO(this DTO.ConflictCondition conflictCondition) => new Types.ConflictCondition(
			conflictCondition.Package,
			conflictCondition.Interface,
			conflictCondition.And.Denull().Select(cond => cond.FromDTO()),
			conflictCondition.Or.Denull().Select(cond => cond.FromDTO())
		);

		public static DTO.ConflictCondition ToDTO(this Types.ConflictCondition conflictCondition) => new DTO.ConflictCondition {
			And = conflictCondition.And.Select(cond => cond.ToDTO()).ToArray(),
			Or = conflictCondition.Or.Select(cond => cond.ToDTO()).ToArray(),
			Interface = conflictCondition.InterfaceName,
			Package = conflictCondition.PackageName,
		};

		//ConflictResolution
		public static Types.ConflictResolution FromDTO(this DTO.ConflictResolution res) => new Types.ConflictResolution(res.Dependencies.FromDTO(), res.Install.FromDTO());

		public static DTO.ConflictResolution ToDTO(this Types.ConflictResolution res) => new DTO.ConflictResolution { Dependencies = res.Dependency.ToDTO(), Install = res.Installation.ToDTO() };

		//DependencyConflictResolution
		public static Types.DependencyConflictResolution FromDTO(this DTO.DependencyConflictResolution res) => new Types.DependencyConflictResolution(res.Force.FromDTO(), res.Decline.FromDTO());

		public static DTO.DependencyConflictResolution ToDTO(this Types.DependencyConflictResolution res) => new DTO.DependencyConflictResolution { Decline = res.Decline.ToDTO(), Force = res.Force.ToDTO() };

		//InstallationConflictResolution
		public static Types.InstallationConflictResolution FromDTO(this DTO.InstallationConflictResolution res) => new Types.InstallationConflictResolution(res.Packages.Denull());

		public static DTO.InstallationConflictResolution ToDTO(this Types.InstallationConflictResolution res) => new DTO.InstallationConflictResolution { Packages = res.PackageNames.ToArray() };

		//DeclinedDependencySet
		public static Types.DeclinedDependencySet FromDTO(this DTO.DeclinedDependency res) => new Types.DeclinedDependencySet(res.Packages.Denull(), res.Interfaces.Denull());

		public static DTO.DeclinedDependency ToDTO(this Types.DeclinedDependencySet res) => new DTO.DeclinedDependency { Interfaces = res.InterfaceNames.ToArray(), Packages = res.PackageNames.ToArray() };

		//ForcedDependencySet
		public static Types.ForcedDependencySet FromDTO(this DTO.ForcedDependency res) => new Types.ForcedDependencySet(res.Packages.Denull(), res.Interfaces.Denull());

		public static DTO.ForcedDependency ToDTO(this Types.ForcedDependencySet res) => new DTO.ForcedDependency { Interfaces = res.InterfaceNames.ToArray(), Packages = res.PackageNames.ToArray() };

		//InterfaceProvision
		public static Types.InterfaceProvision FromDTO(this DTO.InterfaceProvision prov) => new Types.InterfaceProvision(prov.Name, prov.Version.FromDTO());

		public static DTO.InterfaceProvision ToDTO(this Types.InterfaceProvision prov) => new DTO.InterfaceProvision { Name = prov.InterfaceName, Version = prov.Version.ToDTO() };

		//InterfaceDependency
		public static Types.InterfaceDependency FromDTO(this DTO.InterfaceDependency dep) => new Types.InterfaceDependency(dep.Name, dep.Version.FromDTO(), Types.CompatibilitySide.Universal);

		public static DTO.InterfaceDependency ToDTO(this Types.InterfaceDependency dep) => new DTO.InterfaceDependency { Name = dep.InterfaceName, Version = dep.VersionSpec.ToDTO() };

		//PackageDependency
		public static Types.PackageDependency FromDTO(this DTO.PackageDependency dep) => new Types.PackageDependency(dep.Name, dep.Version.FromDTO(), dep.Side.FromDTO());

		public static DTO.PackageDependency ToDTO(this Types.PackageDependency dep) => new DTO.PackageDependency { Name = dep.PackageName, Version = dep.VersionSpec.ToDTO(), Side = dep.Side.ToDTO() };
	}
}
