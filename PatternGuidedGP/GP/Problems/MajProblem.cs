using PatternGuidedGP.GP.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Problems {
	class MajProblem : Problem {
		public MajProblem(int n) {
			TestSuite = new BoolTestSuiteGenerator().Create(n, parameters => {
				int trues = 0;
				foreach (var par in parameters) {
					if ((bool) par) {
						trues++;
					}
					Console.Write((bool)par);
				}
				Console.WriteLine("=>" + (trues > n / 2));
				return trues > n / 2;
			});
		}
	}
}
