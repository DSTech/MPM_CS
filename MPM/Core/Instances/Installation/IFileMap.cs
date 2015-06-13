using System;
using System.Collections;
using System.Collections.Generic;

namespace MPM.Core.Instances.Installation {
	/// <summary>
	/// A mapping of file <see cref="Uri"/>s to <see cref="IEnumerable{IFileOperation}"/>s.
	/// Able to register and unregister operations to URIs
	/// </summary>
	public interface IFileMap : IReadOnlyDictionary<Uri, IEnumerable<IFileOperation>> {
		void Register(Uri uri, string packageId, string operationId, IFileOperation operation);
		bool Unregister(Uri uri, string packageId, string operationId);
	}
}
