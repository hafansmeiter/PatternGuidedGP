using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP.Tests;

namespace PatternGuidedGP.GP.Problems.Advanced {
	class MedianProblem : Problem {
		public override Type RootType => typeof(void);
		public override Type ReturnType => typeof(int);

		public MedianProblem(int n, bool initialize = true) : base(n, initialize) {
		}

		protected override TestSuite GetTestSuite() {
			throw new NotImplementedException();
		}

		protected override void GetCodeTemplate(CodeTemplateBuilder builder) {
			builder.AddParameter(typeof(int), "arr", true)
				.AddParameter(typeof(int), "length", false)
				.SetParameters();
		}

		protected override void GetInstructionSet(InstructionSetBuilder builder) {
			builder.AddIntegerDomain()
				.AddBooleanDomain()
				.AddIfStatement()
				.AddForCountStatement()
				.AddIntTargetVariable()
				.AddIntVariable("arr", true)
				.AddIntVariable("length");
		}
	}
}
