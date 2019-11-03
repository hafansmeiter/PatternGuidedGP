using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Util {
	class Logger {
		public static int Level { get; set; } = 1;
		public static bool WriteToFile { get; set; } = true;
		public static string FileName { get; set; } = "PatternGuidedGP.txt";

		public static void Write(int level, string text) {
			if (level <= Level) {
				Console.Write(text);
				if (WriteToFile) {
					AppendToFile(text);
				}
			}
		}

		public static void WriteLine(int level, string text) {
			Write(level, text + "\n");
		}

		public static void AppendToFile(string text) {
			using (StreamWriter w = File.AppendText(FileName)) {
				w.Write($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToShortDateString()}: ");
				w.Write(text);
			}
		}
	}
}
