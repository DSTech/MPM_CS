using System;
using Autofac;
using PowerArgs;

namespace MPM.CLI {
    public partial class RootArgs {
        [ArgActionMethod]
        [ArgShortcut("auth")]
        public void Login(LoginArgs args) {
            var loginActionProvider = new LoginActionProvider();
            loginActionProvider.Provide(Resolver, args);
        }
    }

    public class LoginActionProvider : IActionProvider<LoginArgs> {
        public void Provide(IContainer resolver, LoginArgs args) {
            throw new NotImplementedException();
        }
    }
}