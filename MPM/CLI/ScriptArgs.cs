using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArgs;

namespace MPM.CLI {
    public class ScriptArgs : ICommandLineArgs {
        [ArgPosition(1)]
        public string ScriptName { get; set; }

        [ArgPosition(2)]
        public List<string> SubArgs { get; set; }
    }
}
