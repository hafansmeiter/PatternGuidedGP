using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP.Tests;

namespace PatternGuidedGP.GP.Problems {
	class MaximumProblem : CodingProblem {
		public override Type ReturnType => typeof(int);
		public override Type ParameterType => typeof(int);

		public MaximumProblem(int parameterCount) : base(parameterCount) {
		}

		protected override TestSuite GetTestSuite() {
			return new IntTestSuiteGenerator().Create(ParameterCount, parameters => {
				int max = (int) parameters[0];
				for (int i = 1; i < parameters.Length; i++) {
					if ((int) parameters[i] > max) {
						max = (int)parameters[i];
					}
				}
				return max;
			});
		}
	}
}
