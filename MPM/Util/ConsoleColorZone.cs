using System;
using System.Reactive.Disposables;

namespace MPM.Util {
    public class ConsoleColorZone : IDisposable {
        readonly IDisposable disposable;

        public ConsoleColorZone(ConsoleColor newColor) {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = newColor;
            disposable = Disposable.Create(() => {
                Console.ForegroundColor = originalColor;
            });
        }

        public static ConsoleColorZone Create(ConsoleColor newColor) => new ConsoleColorZone(newColor);

        public static ConsoleColorZone Error => new ConsoleColorZone(ConsoleColor.Red);
        public static ConsoleColorZone Success => new ConsoleColorZone(ConsoleColor.Green);
        public static ConsoleColorZone Info => new ConsoleColorZone(ConsoleColor.DarkGray);

        #region Implementation of IDisposable

        public void Dispose() {
            this.disposable.Dispose();
        }

        #endregion
    }
}
