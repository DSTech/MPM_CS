using System;
using System.Reactive.Disposables;

namespace MPM.Util {
    public struct ConsoleColorZone : IDisposable {
        readonly bool _changed;
        readonly ConsoleColor originalColor;

        public ConsoleColorZone(ConsoleColor newColor) {
            this.originalColor = Console.ForegroundColor;
            if (originalColor != newColor) {
                Console.ForegroundColor = newColor;
                _changed = true;
            } else {
                _changed = false;
            }
        }

        public static ConsoleColorZone Create(ConsoleColor newColor) => new ConsoleColorZone(newColor);

        public static ConsoleColorZone Error => new ConsoleColorZone(ConsoleColor.Red);
        public static ConsoleColorZone Success => new ConsoleColorZone(ConsoleColor.Green);
        public static ConsoleColorZone Info => new ConsoleColorZone(ConsoleColor.DarkGray);

        #region Implementation of IDisposable

        public void Dispose() {
            if (_changed) {
                Console.ForegroundColor = originalColor;
            }
        }

        #endregion
    }
}
