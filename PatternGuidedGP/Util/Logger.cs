using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Util {
	class Logger {
		public static int Level { get; set; } = 1;

		public static void Write(int level, string text) {
			if (level <= Level) {
				Console.Write(text);
			}
		}

		public static void WriteLine(int level, string text) {
			if (level <= Level) {
				Console.WriteLine(text);
			}
		}
	}
}
