using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.Profiles {
    /// <summary>
    ///     Stores anything user-specific that may be needed by a launcher on a multi-instance basis.
    /// </summary>
    /// <remarks>
    ///     <see cref="Preferences" /> provides any setting that is not necessarily required across all instances but is still
    ///     user-specific,
    ///     such as- for example- non-gameplay-affecting game configuration. Launcher-specific keys should be formatted
    ///     launcherName_preferenceName.
    /// </remarks>
    public interface IProfile {
        string Name { get; }
        IReadOnlyDictionary<string, string> Preferences { get; }
    }
}
