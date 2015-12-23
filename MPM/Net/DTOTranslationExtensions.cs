using System;
using System.Linq;
using MPM.Extensions;
using MPM.Net.DTO;
using MPM.Types;
using semver.tools;
using Author = MPM.Types.Author;
using Build = MPM.Types.Build;
using ConflictCondition = MPM.Types.ConflictCondition;
using ConflictResolution = MPM.Types.ConflictResolution;
using DependencyConflictResolution = MPM.Types.DependencyConflictResolution;
using InstallationConflictResolution = MPM.Types.InstallationConflictResolution;
using InterfaceDependency = MPM.Types.InterfaceDependency;
using InterfaceProvision = MPM.Types.InterfaceProvision;
using Package = MPM.Types.Package;
using PackageDependency = MPM.Types.PackageDependency;
using Version = MPM.Net.DTO.Version;
using VersionSpec = semver.tools.VersionSpec;

namespace MPM.Net {
    public static class DTOTranslationExtensions {
        //CompatibilityPlatform
        public static CompatibilityPlatform FromCompatibilityPlatformDTO(string platformDto)
            => (CompatibilityPlatform)Enum.Parse(typeof(CompatibilityPlatform), platformDto, true);

        public static string ToDTO(this CompatibilityPlatform platform) => platform.ToString();

        //CompatibilitySide
        public static CompatibilitySide FromDTO(this PackageSide side) {
            switch (side) {
                case PackageSide.Client:
                    return CompatibilitySide.Client;
                case PackageSide.Server:
                    return CompatibilitySide.Server;
                case PackageSide.Universal:
                    return CompatibilitySide.Universal;
                default:
                    throw new NotSupportedException();
            }
        }

        public static PackageSide ToDTO(this CompatibilitySide side) {
            switch (side) {
                case CompatibilitySide.Client:
                    return PackageSide.Client;
                case CompatibilitySide.Server:
                    return PackageSide.Server;
                case CompatibilitySide.Universal:
                    return PackageSide.Universal;
                default:
                    throw new NotSupportedException();
            }
        }

        //Arch
        public static Arch FromArchDTO(string arch) => new Arch(arch);

        public static string ToDTO(this Arch arch) => arch.Id;

        //Author
        public static Author FromDTO(this DTO.Author author) => new Author(author.Name, author.Email);

        public static DTO.Author ToDTO(this Author author) => new DTO.Author { Name = author.Name, Email = author.Email };

        //Version
        public static SemanticVersion FromDTO(this Version version)
            => new SemanticVersion(version.Major, version.Minor, version.Patch);

        public static Version ToDTO(this SemanticVersion version) => Version.Parse(version.ToString());

        //VersionSpec
        public static VersionSpec FromDTO(this DTO.VersionSpec version) => new VersionSpec(
            version.Minimum.FromDTO(),
            version.MinInclusive,
            version.Maximum.FromDTO(),
            version.MaxInclusive
            );

        public static DTO.VersionSpec ToDTO(this VersionSpec version) => new DTO.VersionSpec {
            Minimum = version.MinVersion.ToDTO(),
            MinInclusive = version.IsMinInclusive,
            Maximum = version.MaxVersion.ToDTO(),
            MaxInclusive = version.IsMaxInclusive
        };

        //Package
        public static Package FromDTO(this DTO.Package package) => new Package(
            package.Name,
            package.Authors.Denull().Select(author => author.FromDTO()),
            package.Builds.Denull().Select(
                b => {
                    if (b.Package == null) {
                        b.Package = package.Name;
                    }
                    return b;
                }).Select(build => {
                    var _bDto = build.FromDTO();
                    _bDto.Authors = package.Authors.Denull().Select(a => a.FromDTO()).ToList();
                    return _bDto;
                })
            );

        public static DTO.Package ToDTO(this Package package) => new DTO.Package {
            Name = package.Name,
            Authors = package.Authors.Select(author => author.ToDTO()).ToList(),
            Builds = package.Builds.Select(build => {
                var _bDto = build.ToDTO();
                _bDto.Authors = package.Authors.Denull().Select(a => a.ToDTO()).ToList();
                return _bDto;
            }).ToList(),
        };

        //Build
        public static Build FromDTO(this DTO.Build build) => new Build(
            build.Package,
            build.Authors.Denull().Select(author => author.FromDTO()).ToList(),
            build.Version.FromDTO(),
            build.GivenVersion,
            FromArchDTO(build.Arch),
            build.Platform != null ? FromCompatibilityPlatformDTO(build.Platform) : CompatibilityPlatform.Universal,
            FromDTO(build.Side),
            build.Interfaces.Denull().Select(interfaceProvision => interfaceProvision.FromDTO()),
            build.Dependencies?.Interfaces.Denull().Select(interfaceDependency => interfaceDependency.FromDTO()),
            build.Dependencies?.Packages.Denull().Select(packageDependency => packageDependency.FromDTO()),
            build.Conflicts.Denull().Select(packageConflict => packageConflict.FromDTO()),
            build.Hashes.Denull().Select(hash => Hash.Parse(hash)),
            build.Stable
            );

        public static DTO.Build ToDTO(this Build build) => new DTO.Build {
            Package = build.PackageName,
            Arch = build.Arch.ToDTO(),
            Conflicts = build.Conflicts.Select(conflict => conflict.ToDTO()).ToList(),
            Interfaces = build.InterfaceProvisions.Select(interfaceProvision => interfaceProvision.ToDTO()).ToList(),
            Dependencies = new DependencySpec {
                Interfaces = build.InterfaceDependencies.Select(interfaceDependency => interfaceDependency.ToDTO()).ToList(),
                Packages = build.PackageDependencies.Select(packageDependency => packageDependency.ToDTO()).ToList()
            },
            GivenVersion = build.GivenVersion,
            Hashes = build.Hashes.Denull().Select(hash => hash.ToString()).ToList(),
            Platform = build.Platform.ToDTO(),
            Side = build.Side.ToDTO(),
            Stable = build.Stable,
            Version = build.Version.ToDTO()
        };

        //PackageConflict
        public static Conflict FromDTO(this PackageConflict conflict)
            => new Conflict(conflict.Condition.FromDTO(), conflict.Resolution.FromDTO());

        public static PackageConflict ToDTO(this Conflict conflict)
            => new PackageConflict { Condition = conflict.Condition.ToDTO(), Resolution = conflict.Resolution.ToDTO() };

        //ConflictCondition
        public static ConflictCondition FromDTO(this DTO.ConflictCondition conflictCondition) => new ConflictCondition(
            conflictCondition.Package,
            conflictCondition.Interface,
            conflictCondition.And.Denull().Select(cond => cond.FromDTO()),
            conflictCondition.Or.Denull().Select(cond => cond.FromDTO())
            );

        public static DTO.ConflictCondition ToDTO(this ConflictCondition conflictCondition) => new DTO.ConflictCondition {
            And = conflictCondition.And.Select(cond => cond.ToDTO()).ToList(),
            Or = conflictCondition.Or.Select(cond => cond.ToDTO()).ToList(),
            Interface = conflictCondition.InterfaceName,
            Package = conflictCondition.PackageName
        };

        //ConflictResolution
        public static ConflictResolution FromDTO(this DTO.ConflictResolution res)
            => new ConflictResolution(res.Dependencies.FromDTO(), res.Install.FromDTO());

        public static DTO.ConflictResolution ToDTO(this ConflictResolution res)
            => new DTO.ConflictResolution { Dependencies = res.Dependency.ToDTO(), Install = res.Installation.ToDTO() };

        //DependencyConflictResolution
        public static DependencyConflictResolution FromDTO(this DTO.DependencyConflictResolution res)
            => new DependencyConflictResolution(res.Force.FromDTO(), res.Decline.FromDTO());

        public static DTO.DependencyConflictResolution ToDTO(this DependencyConflictResolution res)
            => new DTO.DependencyConflictResolution { Decline = res.Decline.ToDTO(), Force = res.Force.ToDTO() };

        //InstallationConflictResolution
        public static InstallationConflictResolution FromDTO(this DTO.InstallationConflictResolution res)
            => new InstallationConflictResolution(res.Packages.Denull());

        public static DTO.InstallationConflictResolution ToDTO(this InstallationConflictResolution res)
            => new DTO.InstallationConflictResolution { Packages = res.PackageNames.ToList() };

        //DeclinedDependencySet
        public static DeclinedDependencySet FromDTO(this DeclinedDependency res)
            => new DeclinedDependencySet(res.Packages.Denull(), res.Interfaces.Denull());

        public static DeclinedDependency ToDTO(this DeclinedDependencySet res)
            => new DeclinedDependency { Interfaces = res.InterfaceNames.ToList(), Packages = res.PackageNames.ToList() };

        //ForcedDependencySet
        public static ForcedDependencySet FromDTO(this ForcedDependency res)
            => new ForcedDependencySet(res.Packages.Denull(), res.Interfaces.Denull());

        public static ForcedDependency ToDTO(this ForcedDependencySet res)
            => new ForcedDependency { Interfaces = res.InterfaceNames.ToList(), Packages = res.PackageNames.ToList() };

        //InterfaceProvision
        public static InterfaceProvision FromDTO(this DTO.InterfaceProvision prov)
            => new InterfaceProvision(prov.Name, prov.Version.FromDTO());

        public static DTO.InterfaceProvision ToDTO(this InterfaceProvision prov)
            => new DTO.InterfaceProvision { Name = prov.InterfaceName, Version = prov.Version.ToDTO() };

        //InterfaceDependency
        public static InterfaceDependency FromDTO(this DTO.InterfaceDependency dep)
            => new InterfaceDependency(dep.Name, dep.Version.FromDTO(), CompatibilitySide.Universal);

        public static DTO.InterfaceDependency ToDTO(this InterfaceDependency dep)
            => new DTO.InterfaceDependency { Name = dep.InterfaceName, Version = dep.VersionSpec.ToDTO() };

        //PackageDependency
        public static PackageDependency FromDTO(this DTO.PackageDependency dep)
            => new PackageDependency(dep.Name, dep.Version.FromDTO(), dep.Side.FromDTO());

        public static DTO.PackageDependency ToDTO(this PackageDependency dep)
            => new DTO.PackageDependency { Name = dep.PackageName, Version = dep.VersionSpec.ToDTO(), Side = dep.Side.ToDTO() };
    }
}
