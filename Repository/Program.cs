using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository {
    public static class Program {
        public static void Main(string[] args) {
            using (var host = new AppHost()) {
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
