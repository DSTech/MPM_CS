using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using MPM.Core;
using PowerArgs;

namespace MPM.CLI {
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public partial class RootArgs : ICommandLineArgs {
        [ArgIgnore]
        public IContainer Resolver { get; set; }

        [ArgDescription("Prints all messages to stdout")]
        [ArgShortcut("--verbose"), ArgShortcut("-v")]
        [ArgEnforceCase]
        public bool Verbose { get; set; }

        [HelpHook]
        [ArgShortcut("--help"), ArgShortcut("-h")]
        [ArgEnforceCase]
        public bool Help { get; set; }
    }
}
