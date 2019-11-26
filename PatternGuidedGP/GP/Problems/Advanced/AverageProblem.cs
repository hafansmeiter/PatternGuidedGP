using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP.Tests;

namespace PatternGuidedGP.GP.Problems.Advanced {
	class AverageProblem : Problem {
		public override Type RootType => typeof(void);
		public override Type ReturnType => typeof(float);

		public AverageProblem(int n, bool initialize = true) : base(n, initialize) {
		}

		protected override TestSuite GetTestSuite() {
			throw new NotImplementedException();
		}

		protected override void GetCodeTemplate(CodeTemplateBuilder builder) {
			builder.AddParameter(typeof(float), "arr", true)
				.AddParameter(typeof(int), "length", false)
				.SetParameters();
		}

		protected override void GetInstructionSet(InstructionSetBuilder builder) {
			builder.AddFloatDomain()
				.AddBooleanDomain()
				.AddIfStatement()
				.AddForCountStatement()
				.AddFloatTargetVariable()
				.AddFloatVariable("arr", true)
				.AddIntVariable("length");
		}
	}
}
