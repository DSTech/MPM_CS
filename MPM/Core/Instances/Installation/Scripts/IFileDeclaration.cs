using System;
using System.Collections.Generic;
using MPM.Core.Instances.Cache;
using MPM.Core.Protocols;
using semver.tools;

namespace MPM.Core.Instances.Installation.Scripts {
	public interface IFileDeclaration {
		/// <summary>
		/// The name of the package that sourced this declaration.
		/// </summary>
		String PackageName { get; }
		/// <summary>
		/// The version of the package which sourced this declaration.
		/// </summary>
		SemanticVersion PackageVersion { get; }
		/// <summary>
		/// The path of the file to be installed relative to the root of the package archive root.
		/// The first portion may be a "protocol", eg https://. Protocols require a <see cref="Hash"/> to be specified.
		/// May be null in sourceless declarations.
		/// </summary>
		String Source { get; }
		/// <summary>
		/// Must be specified if <see cref="Source"/> is a protocol source. May be null otherwise.
		/// </summary>
		Byte[] Hash { get; }
		/// <summary>
		/// A description for the operation; May be null.
		/// </summary>
		String Description { get; }
		/// <summary>
		/// A set of targets for the declaration.
		/// May be null only if the operation does not have a specific target.
		/// May be an empty collection for declarations which support multiple targets.
		/// </summary>
		IReadOnlyCollection<String> Targets { get; }
		/// <summary>
		/// Should place any required resources for the declaration into the cache if they do not exist.
		/// Should prepare any information required for the generation of file operations.
		/// Must pass Hash to calls to ProtocolResolver.
		/// Should store resolved items into the cache.
		/// Must be called before any calls to <see cref="GenerateOperations"/> may occur.
		/// </summary>
		/// <param name="packageCachedName">The cached name of the package from which this instruction was loaded.</param>
		/// <param name="cacheManager">
		/// The cache which must have any necessary non-package resources added under a uniquely identifiable name.
		/// Contains this instance's package archive as an entry under the name stored in <paramref name="packageCachedName"/>.
		/// </param>
		/// <param name="protocolResolver">A resolver provided to allow fetching of resources if they are not in the cache.</param>
		void EnsureCached(string packageCachedName, ICacheManager cacheManager, IProtocolResolver protocolResolver);
		/// <summary>
		/// Creates entries for use in an <see cref="IFileMap"/> associated to this particular instruction.
		/// <see cref="EnsureCached(string, ICacheManager, IProtocolResolver)"/> must be called before this method may be used.
		/// </summary>
		/// <returns>
		/// A relation of filepaths and their associated <see cref="IFileOperation"/> sequences.
		/// </returns>
		IReadOnlyDictionary<String, IReadOnlyCollection<IFileOperation>> GenerateOperations();
	}
}
