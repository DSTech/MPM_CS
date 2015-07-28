using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MPM;
using MPM.Types;
using semver.tools;

namespace MPM.Types.Mapping {

	public class DTOMappings : Profile {
		public override string ProfileName => nameof(DTOMappings);

		protected override void Configure() {
			base.Configure();

			CreateMap<string, Arch>().ConstructUsing(str => new Arch(str));
			CreateMap<Arch, string>().ConvertUsing(arch => arch.ToString());

			CreateMap<Net.DTO.ConflictCondition, ConflictCondition>()
				.Helper()
				.OneToOne(cnd => cnd.InterfaceName, cnd => cnd.Interface)
				.OneToOne(cnd => cnd.PackageName, cnd => cnd.Package)
				.ApplyTwoWay();

			CreateMap<Net.DTO.ConflictResolution, ConflictResolution>()
				.Helper()
				.OneToOne(dst => dst.Dependency, src => src.Dependencies)
				.OneToOne(dst => dst.Installation, src => src.Install)
				.ApplyTwoWay();

			CreateMap<Net.DTO.InstallationConflictResolution, InstallationConflictResolution>()
				.Helper()
				.OneToOne(dst => dst.PackageNames, src => src.Packages)
				.ApplyTwoWay();

			CreateMap<Net.DTO.DependencyConflictResolution, DependencyConflictResolution>()
				.Helper()
				.OneToOne(dst => dst.Force, src => src.Decline)
				.OneToOne(dst => dst.Decline, src => src.Force)
				.ApplyTwoWay();

			CreateMap<Net.DTO.ForcedDependency, ForcedDependencySet>()
				.Helper()
				.OneToOne(dst => dst.InterfaceNames, src => src.Interfaces)
				.OneToOne(dst => dst.PackageNames, src => src.Packages)
				.ApplyTwoWay();

			CreateMap<Net.DTO.DeclinedDependency, DeclinedDependencySet>()
				.Helper()
				.OneToOne(dst => dst.InterfaceNames, src => src.Interfaces)
				.OneToOne(dst => dst.PackageNames, src => src.Packages)
				.ApplyTwoWay();

			CreateMap<Net.DTO.InterfaceDependency, InterfaceDependency>()
				.Helper()
				.OneToOne(dst => dst.InterfaceName, src => src.Name)
				.OneToOne(dst => dst.VersionSpec, src => src.Version)
				.ApplyTwoWay(
					to => to.ForMember((InterfaceDependency dst) => dst.Side, opt => opt.UseValue(CompatibilitySide.Universal)),
					from => from.Helper().IgnoreSource(src => src.Side).Expression
				);

			CreateMap<Net.DTO.PackageDependency, PackageDependency>()
				.Helper()
				.OneToOne(dst => dst.PackageName, src => src.Name)
				.OneToOne(dst => dst.VersionSpec, src => src.Version)
				.OneToOne(dst => dst.Side, src => src.Side)
				.ApplyTwoWay();

			CreateMap<Net.DTO.PackageConflict, Conflict>()
				.Helper()
				.OneToOne(dst => dst.Condition, src => src.Condition)
				.OneToOne(dst => dst.Resolution, src => src.Resolution)
				.ApplyTwoWay();

			CreateMap<Net.DTO.InterfaceProvision, InterfaceProvision>()
				.Helper()
				.OneToOne(dst => dst.InterfaceName, src => src.Name)
				.OneToOne(dst => dst.Version, src => src.Version)
				.ApplyTwoWay();

			CreateMap<string, SemanticVersion>().ConvertUsing(SemanticVersion.ParseNuGet);
			CreateMap<SemanticVersion, string>().ConvertUsing(v => v.ToString());

			CreateMap<string, VersionSpec>().ConvertUsing(v => (VersionSpec)(VersionSpec.ParseNuGet(v)));
			CreateMap<VersionSpec, string>().ConvertUsing(v => v.ToStringNuGet());
			CreateMap<IVersionSpec, string>().ConvertUsing(v => ((VersionSpec)v).ToStringNuGet());

			CreateMap<Net.DTO.Version, SemanticVersion>().ConvertUsing(v => new SemanticVersion(v.Major, v.Minor, v.Patch));
			CreateMap<SemanticVersion, Net.DTO.Version>().ConvertUsing(v => Net.DTO.Version.Parse(v.ToString()));

			CreateMap<Net.DTO.VersionSpec, VersionSpec>()
				.ConstructUsing((Net.DTO.VersionSpec vspec) => new VersionSpec(
					new SemanticVersion(vspec.Minimum.Major, vspec.Minimum.Minor, vspec.Minimum.Patch),
					vspec.MinInclusive,
					new SemanticVersion(vspec.Maximum.Major, vspec.Maximum.Minor, vspec.Maximum.Patch),
					vspec.MaxInclusive
				))
				.ForAllMembers(opts => opts.Ignore());

			CreateMap<VersionSpec, Net.DTO.VersionSpec>()
				.Helper()
				.OneToOne(dst => dst.Minimum, src => src.MinVersion)
				.OneToOne(dst => dst.MinInclusive, src => src.IsMinInclusive)
				.OneToOne(dst => dst.Maximum, src => src.MaxVersion)
				.OneToOne(dst => dst.MaxInclusive, src => src.IsMaxInclusive)
				.Apply();

			CreateMap<Build, Net.DTO.DependencySpec>()
				.Helper()
				.OneToOne(dst => dst.Interfaces, src => src.InterfaceDependencies)
				.OneToOne(dst => dst.Packages, src => src.PackageDependencies)
				.Apply();

			CreateMap<Net.DTO.Build, Build>(MemberList.Destination)
				.Helper()
				.OneToOne(dst => dst.PackageName, src => src.Package)
				.OneToOne(dst => dst.InterfaceProvisions, src => src.Interfaces)
				.ApplyTwoWay(
					to => to.Helper()
						.ApplyOneWay(dst => dst.InterfaceDependencies, src => src.Dependencies.Interfaces)
						.ApplyOneWay(dst => dst.PackageDependencies, src => src.Dependencies.Packages)
						.Expression
					,
					from => from.Helper()
						.ApplyOneWay(dst => dst.Dependencies, src => src)
						.IgnoreSource(src => src.InterfaceDependencies)
						.IgnoreSource(src => src.PackageDependencies)
						.Expression
				);
		}
	}
}
