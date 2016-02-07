using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceKit.Logging;
using NServiceKit.ServiceInterface.Admin;
using NServiceKit.ServiceInterface.Providers;
using NServiceKit.Text;

namespace Repository {
    public static class Program {
        public static void Main(string[] args) {
            LogManager.LogFactory = new NServiceKit.Logging.Support.Logging.ConsoleLogFactory();
            using (var host = new AppHost()) {
                host.PreRequestFilters.Insert(0, (httpReq, httpRes) => {
                    httpReq.UseBufferedStream = true;
                });
                host.Config.DebugMode = true;
                host.Plugins.Add(new RequestLogsFeature() { RequiredRoles = new string[0] });
                host.Init();
                host.Start("http://*:3000/");
                Console.WriteLine("Press Q to exit.");
                var shouldExit = false;
                do {
                    shouldExit = Console.ReadKey(true).Key == ConsoleKey.Q;
                } while (!shouldExit);
                host.Stop();
            }
        }
    }
}
