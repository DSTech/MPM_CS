using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MPM.Core.Dependency;
using MPM.Core.Instances.Cache;
using Platform.VirtualFileSystem;
using semver.tools;

namespace MPM.Core.Instances.Installation {
    /// <summary>
    ///     A single operation upon a specific filepath.
    /// </summary>
    /// <remarks>
    ///     Lossy operations must not be reversible.
    /// </remarks>
    public interface IFileOperation {
        /// <summary>
        ///     Name of the package that created the operation.
        /// </summary>
        String PackageName { get; }

        /// <summary>
        ///     Version of the package that created the operation.
        /// </summary>
        SemanticVersion PackageVersion { get; }

        /// <summary>
        ///     Performs the operation upon the specified path within the given filesystem.
        /// </summary>
        /// <remarks>
        ///     Reversible entries must have <see cref="UsesPreviousContents" /> as true.
        /// </remarks>
        /// <param name="fileSystem">The filesystem upon which the operations should be performed.</param>
        /// <param name="path">The path of the content which should be altered.</param>
        void Perform(IFileSystem fileSystem, String path, ICacheReader cache);
    }
}
