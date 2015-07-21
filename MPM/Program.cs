using System;
using PowerArgs;

namespace MPM.CLI {
	public class Program {
		public static void Main(string[] args) {
			ArgAction<LaunchArgs> parsed;
			try {
				//parsed = Args.InvokeAction<LaunchArgs>(args);
				parsed = Args.ParseAction<LaunchArgs>(args);
			} catch (ArgException ex) {
				Console.WriteLine(ex.Message);
				Console.WriteLine(ArgUsage.GenerateUsageFromTemplate<LaunchArgs>());
				return;
			}
			if (parsed?.Args == null) {
				return;
			}
			try {
				parsed.Invoke();
			} catch (ArgException ex) {
				Console.WriteLine(ex.Message);
				Console.WriteLine(ArgUsage.GenerateUsageFromTemplate<LaunchArgs>());
				return;
			}
		}
	}
}
