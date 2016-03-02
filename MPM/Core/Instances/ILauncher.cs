using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using MPM.Core.Profiles;

namespace MPM.Core.Instances {
    public interface ILauncher {
        void Launch(IContainer resolver, Instance instance, IProfile profile);
    }
}
