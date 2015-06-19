using System;
using System.Collections;
using System.Collections.Generic;
using semver.tools;

namespace MPM.Core.Instances.Installation {
	/// <summary>
	/// A mapping of file path <see cref="String"/>s to <see cref="IReadOnlyCollection{IFileOperation}"/>s.
	/// Able to register and unregister operations to paths
	/// </summary>
	public interface IFileMap : IReadOnlyDictionary<String, IReadOnlyCollection<IFileOperation>> {
		void Register(String path, IFileOperation operation);
		bool Unregister(String path, string packageName, SemanticVersion packageVersion);
		bool UnregisterPackage(string packageName);
		bool UnregisterPackage(String path, string packageName);
	}
}
