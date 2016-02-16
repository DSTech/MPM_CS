using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Util {
    public static class TimerUtil {
        public static TimeSpan Time(Action action) {
            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
        public static TimeSpan Time<T>(out T toInitialize, Action action) {
            var stopwatch = Stopwatch.StartNew();
            toInitialize = default(T);
            action();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }
}
