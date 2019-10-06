using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP.Tests;

namespace PatternGuidedGP.GP.Problems {
	class IsOrderedProblem : CodingProblem {
		public override Type ReturnType => typeof(bool);
		public override Type ParameterType => typeof(int);

		public IsOrderedProblem(int parameterCount) : base(parameterCount) {
		}

		protected override TestSuite GetTestSuite() {
			return new IntTestSuiteGenerator().Create(ParameterCount, parameters => {
				for (int i = 0; i < parameters.Length - 1; i++) {
					if ((int) parameters[i] > (int) parameters[i + 1]) {
						return false;
					}
				}
				return true;
			});
		}
	}
}
