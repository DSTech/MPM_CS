using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Extensions;
using MPM.Net.Protocols.Minecraft.Types;

namespace MPM.Net.Protocols.Minecraft {

	public static class DTOTranslationExtensions {

		//MinecraftVersion
		public static MinecraftVersion FromDTO(this DTO.MinecraftVersion dto) {
			return new MinecraftVersion(
				dto.Id,
				DateTime.ParseExact(dto.ReleaseTime ?? dto.Time, "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz", System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime(),
				dto.Type == "release" ? ReleaseType.Release : ReleaseType.Snapshot,
				dto.MinecraftArguments,
				dto.MinimumLauncherVersion,
				dto.Assets,
				dto.Libraries.Denull().Select(library => library.FromDTO())
			);
		}

		public static DTO.MinecraftVersion ToDTO(this MinecraftVersion obj) {
			var releaseTime = new DateTimeOffset(DateTime.Parse("2015-08-05T12:22:42+00:00").ToUniversalTime()).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'sszzz");
			return new DTO.MinecraftVersion {
				Id = obj.Id,
				Time = releaseTime,
				ReleaseTime = releaseTime,
				Type = (obj.Type == ReleaseType.Release ? "release" : "snapshot"),
				MinecraftArguments = obj.MinecraftArguments,
				MinimumLauncherVersion = obj.MinimumLauncherVersion,
				Assets = obj.AssetsIdentifier,
				Libraries = obj.Libraries.Select(library => library.ToDTO()).ToArray(),
			};
		}

		//MinecraftVersionListing
		public static MinecraftVersionListing FromDTO(this DTO.MinecraftVersionListing dto) {
			return new MinecraftVersionListing(
				dto.Id,
				DateTime.ParseExact(dto.ReleaseTime ?? dto.Time, "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz", System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime(),
				dto.Type == "snapshot" ? ReleaseType.Snapshot : ReleaseType.Release
			);
		}

		public static DTO.MinecraftVersionListing ToDTO(this MinecraftVersionListing obj) {
			var releaseTime = new DateTimeOffset(DateTime.Parse("2015-08-05T12:22:42+00:00").ToUniversalTime()).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'sszzz");
			return new DTO.MinecraftVersionListing {
				Id = obj.Id,
				Time = releaseTime,
				ReleaseTime = releaseTime,
				Type = (obj.Type == ReleaseType.Snapshot ? "snapshot" : "release"),
			};
		}


		//MinecraftVersionCollection
		public static MinecraftVersionCollection FromDTO(this DTO.MinecraftVersionCollection dto) {
			var versions = dto.Versions.Denull().Select(version => version.FromDTO()).ToArray();
			var latestRelease = versions.First(version => version.Id == dto.Latest.Release);
			var latestSnapshot = versions.First(version => version.Id == dto.Latest.Snapshot);
			return new MinecraftVersionCollection(
				latestRelease,
				latestSnapshot,
				versions
			);
		}

		public static DTO.MinecraftVersionCollection ToDTO(this MinecraftVersionCollection obj) {
			return new DTO.MinecraftVersionCollection {
				Latest = new DTO.MinecraftVersionLatestSpec {
					Release = obj.LatestRelease.Id,
					Snapshot = obj.LatestSnapshot.Id,
				},
				Versions = obj.Versions.Select(version => version.ToDTO()).ToArray(),
			};
		}


		//LibrarySpec
		public static LibrarySpec FromDTO(this DTO.LibrarySpec dto) {
			var splitName = dto.Name.Split(new[] { ':' }, 3);
			return new LibrarySpec(
				package: splitName[0],
				name: splitName[1],
				version: splitName[2],
				extractSpec: dto.Extract.FromDTO(),
				nativesSpec: dto.Natives.FromDTO(),
				rules: dto.Rules.Select(rule => rule.FromDTO())
			);
		}

		public static DTO.LibrarySpec ToDTO(this LibrarySpec obj) {
			return new DTO.LibrarySpec {
				Name = $"{obj.Package}:{obj.Name}:{obj.Version}",
				Extract = obj.Extract.ToDTO(),
				Natives = obj.Natives.ToDTO(),
				Rules = obj.Rules.Select(rule => rule.ToDTO()).ToArray(),
			};
		}


		//LibraryExtractSpec
		public static LibraryExtractSpec FromDTO(this DTO.LibraryExtractSpec dto) {
			if (dto == null) {
				return null;
			}
			return new LibraryExtractSpec(
				dto.Exclude.Denull()
			);
		}

		public static DTO.LibraryExtractSpec ToDTO(this LibraryExtractSpec obj) {
			if (obj == null) {
				return null;
			}
			return new DTO.LibraryExtractSpec {
				Exclude = obj.Exclusions.ToArray(),
			};
		}


		//LibraryNativesSpec
		public static LibraryNativesSpec FromDTO(this DTO.LibraryNativesSpec dto) {
			if (dto == null) {
				return null;
			}
			return new LibraryNativesSpec(
				dto.Windows,
				dto.Linux,
				dto.Osx
			);
		}

		public static DTO.LibraryNativesSpec ToDTO(this LibraryNativesSpec obj) {
			if (obj == null) {
				return null;
			}
			return new DTO.LibraryNativesSpec {
				Windows = obj.Windows,
				Linux = obj.Linux,
				Osx = obj.Osx,
			};
		}


		//LibraryRuleSpec
		public static LibraryRuleSpec FromDTO(this DTO.LibraryRuleSpec dto) {
			if (dto == null) {
				return null;
			}
			return new LibraryRuleSpec(
				dto.Action == "allow" ? LibraryRuleAction.Allow : LibraryRuleAction.Disallow,
				dto.Os.FromDTO()
			);
		}

		public static DTO.LibraryRuleSpec ToDTO(this LibraryRuleSpec obj) {
			if (obj == null) {
				return null;
			}
			return new DTO.LibraryRuleSpec {
				Action = (obj.Action == LibraryRuleAction.Allow ? "allow" : "disallow"),
				Os = obj.Filters.ToDTO(),
			};
		}


		//IEnumerable<LibraryRuleFilter> / DTO.LibraryRuleOSSpec
		public static IEnumerable<LibraryRuleFilter> FromDTO(this DTO.LibraryRuleOSSpec dto) {
			if (dto == null) {
				return null;
			}
			var filters = new List<LibraryRuleFilter>();
			if (dto.Name != null) {
				filters.Add(new LibraryRuleFilterOS(dto.Name));
			}
			if (dto.Version != null) {
				filters.Add(new LibraryRuleFilterOSVersion(dto.Version));
			}
			return filters.ToArray();
		}

		public static DTO.LibraryRuleOSSpec ToDTO(this IEnumerable<LibraryRuleFilter> obj) {
			if (obj == null) {
				return null;
			}
			string name = null;
			{
				var osFilter = (LibraryRuleFilterOS)obj.FirstOrDefault(rule => rule is LibraryRuleFilterOS);
				if (osFilter != null) {
					name = osFilter.OS;
				}
			}
			string version = null;
			{
				var osVersionFilter = (LibraryRuleFilterOSVersion)obj.FirstOrDefault(rule => rule is LibraryRuleFilterOSVersion);
				if (osVersionFilter != null) {
					version = osVersionFilter.Version;
				}
			}
			return new DTO.LibraryRuleOSSpec {
				Name = name,
				Version = version,
			};
		}
	}
}
