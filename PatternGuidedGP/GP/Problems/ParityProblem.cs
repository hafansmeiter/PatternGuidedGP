using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.GP.Tests;

namespace PatternGuidedGP.GP.Problems {
	class ParityProblem : CodingProblem {
		public override Type ReturnType => typeof(bool);
		public override Type ParameterType => typeof(bool);

		public ParityProblem(int n) : base(n) {
		}

		protected override TestSuite GetTestSuite() {
			return new BoolTestSuiteGenerator().Create(ParameterCount, parameters => {
				int trues = 0;
				foreach (var par in parameters) {
					if ((bool)par) {
						trues++;
					}
				}
				return trues % 2 != 0;
			});
		}
	}
}
