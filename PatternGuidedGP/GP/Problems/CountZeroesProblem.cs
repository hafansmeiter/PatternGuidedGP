using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP.Tests;

namespace PatternGuidedGP.GP.Problems {
	class CountZeroesProblem : ScalarCodingProblem {
		public override Type ReturnType => typeof(int);
		public override Type ParameterType => typeof(int);

		public CountZeroesProblem(int parameterCount) : base(parameterCount) {
		}

		protected override TestSuite GetTestSuite() {
			return new IntTestSuiteGenerator().Create(ParameterCount, parameters => {
				int zeroes = 0;
				for (int i = 0; i < parameters.Length; i++) {
					if ((int) parameters[i] == 0) {
						zeroes++;
					}
				}
				return zeroes;
			});
		}
	}
}
