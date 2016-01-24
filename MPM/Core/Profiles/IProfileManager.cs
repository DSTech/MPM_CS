using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.Profiles {
    /// <summary>
    ///     Allows management of <see cref="IProfile" />s within a profile store.
    /// </summary>
    public interface IProfileManager : IProfileReader, IProfileWriter {
    }
}
