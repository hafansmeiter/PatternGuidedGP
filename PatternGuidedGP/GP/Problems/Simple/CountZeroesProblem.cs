using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP.Problems.Simple;
using PatternGuidedGP.GP.Tests;

namespace PatternGuidedGP.GP.Problems.Simple {
	// Use parameters 1,2,3 instead of 0,1,2 in order to reduce 
	// occurrence of DivideByZeroExceptions => count ones
	class CountZeroesProblem : SimpleCodingProblem {
		public override Type ReturnType => typeof(int);
		public override Type ParameterType => typeof(int);

		public CountZeroesProblem(int parameterCount) : base(parameterCount) {
		}

		protected override TestSuite GetTestSuite() {
			return new IntTestSuiteGenerator().Create(ParameterCount, parameters => {
				int zeroes = 0;
				for (int i = 0; i < parameters.Length; i++) {
					if ((int) parameters[i] == 1) {
						zeroes++;
					}
				}
				return zeroes;
			});
		}
	}
}
