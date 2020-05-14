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
			using (StreamWriter w = File.AppendText(FileName + ".txt")) {
				w.Write(text);
			}
		}

		public static void WriteStatisticsHeader() {
			WriteStatisticsLine("gen;total;for;if;and;or;mod;one;two;equals;ret;i;values;length;n;a;b;c;d;<;<=;>;>=;backprop_cx_att;backprop_cx_success;backprop_cx_change;backprop_mut_att;backprop_mut_success;backprop_mut_change;rec_att;rec_success;rec_failure;rec_change");
		}

		public static void WriteStatistics(int generation, params string [] nodeTypes) {
			WriteStatisticsLine(generation + ";" + Statistics.Instance.ToString(nodeTypes));
		}

		private static void WriteStatisticsLine(string line) {
			using (StreamWriter w = File.AppendText(FileName + ".stats.txt")) {
				w.WriteLine(line);
			}
		}
	}
}
