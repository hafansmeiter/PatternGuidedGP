using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.GP.Tests;

namespace PatternGuidedGP.GP.Problems {
	class CompareProblem : ScalarCodingProblem {
		public override Type ReturnType => typeof(bool);
		public override Type ParameterType => typeof(bool);

		public CompareProblem(int n) : base(n) {
		}

		protected override TestSuite GetTestSuite() {
			return new BoolTestSuiteGenerator().Create(ParameterCount, parameters => {
				int firstValue = 0;
				int secondValue = 0;
				int diff = ParameterCount / 2;
				for (int i = 0; i < diff; i++) {
					firstValue <<= 1;
					secondValue <<= 1;
					firstValue += ((bool)parameters[i]) ? 1 : 0;
					secondValue += ((bool)parameters[i + diff]) ? 1 : 0;
				}
				return secondValue < firstValue;
			});
		}
	}
}
