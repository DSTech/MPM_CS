using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceKit.CacheAccess;
using NServiceKit.CacheAccess.Providers;
using NServiceKit.Configuration;
using NServiceKit.Logging;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.Admin;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.ServiceInterface.Providers;
using NServiceKit.Text;

namespace Repository {
    public static class Program {
        public static void Main(string[] args) {
            var appSettings = new AppSettings();
            LogManager.LogFactory = new NServiceKit.Logging.Support.Logging.ConsoleLogFactory();
            var userRepository = new InMemoryAuthRepository();
            var adminPassword = appSettings.GetString("adminPassword");
            if (args.Length >= 1) {
                adminPassword = args[0];
            }
            if (adminPassword != null) {
                var newUser = userRepository.CreateUserAuth(new UserAuth {
                    UserName = "admin",
                }, adminPassword);
                userRepository.SaveUserAuth(newUser);
                Console.WriteLine("Admin account added from settings.");
            }
            var port = appSettings.Get<int>("port", 3000);
            if (args.Length >= 2) {
                port = Int32.Parse(args[1]);
            }
            using (var host = new AppHost()) {
                host.PreRequestFilters.Insert(0, (httpReq, httpRes) => {
                    httpReq.UseBufferedStream = true;
                });
                host.Config.DebugMode = true;
                host.Plugins.Add(new AuthFeature(createSession, new IAuthProvider[] { new BasicAuthProvider() }, "/builds"));
                host.Plugins.Add(new RequestLogsFeature() { RequiredRoles = new string[0] });
                host.Container.Register<ICacheClient>(new MemoryCacheClient());
                host.Container.Register<IUserAuthRepository>(userRepository);
                {
                    host.Init();
                    host.Start($"http://*:{port}/");
                    Console.WriteLine($"Listening on port {port}.");
                    Console.WriteLine("Press Q to exit.");
                    var shouldExit = false;
                    do {
                        shouldExit = Console.ReadKey(true).Key == ConsoleKey.Q;
                    } while (!shouldExit);
                    host.Stop();
                }
            }
        }

        private static IAuthSession createSession() {
            return new AuthUserSession();
        }
    }
}
