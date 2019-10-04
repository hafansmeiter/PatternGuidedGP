using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP.Tests;

namespace PatternGuidedGP.GP.Problems {
	class AllEqualProblem : CodingProblem {

		public AllEqualProblem(int n) : base(n) {
		}

		protected override void AddTreeNodes(TreeNodeRepository repository) {
			base.AddTreeNodes(repository);
			for (int i = 0; i < _parameterCount; i++) {
				repository.Add(new IntIdentifierExpression(((char)('a' + i)).ToString()));
			}
			repository.Add(new BoolIdentifierExpression("ret"),
				new IntAssignmentStatement(),
				new BoolAssignmentStatement());
		}

		protected override TestSuite GetTestSuite() {
			return new IntTestSuiteGenerator().Create(_parameterCount, parameters => {
				int value = (int) parameters[0];
				for (int i = 1; i < parameters.Length; i++) {
					if (value != (int) parameters[i]) {
						return false;
					}
				}
				return true;
			});
		}
	}
}
