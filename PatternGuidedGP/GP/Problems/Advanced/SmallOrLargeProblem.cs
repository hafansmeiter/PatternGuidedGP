using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Util;

namespace PatternGuidedGP.GP.Problems.Advanced {
	class SmallOrLargeProblem : CodingProblem {
		public override IFitnessCalculator FitnessCalculator => new EqualityFitnessCalculator();
		public override Type ReturnType => typeof(string); // not used, result is the printed string

		public int UpperBoundValue { get; set; } = 10000;
		public int LowerBoundValue { get; set; } = -10000;

		protected override TestSuite GetTestSuite() {
			TestSuite testSuite = new TestSuite();
			for (int i = 980; i < 1020; i++) {
				testSuite.TestCases.Add(new TestCase(new object[] { i }, i < 1000 ? "small" : ""));
			}
			for (int i = 1980; i < 2020; i++) {
				testSuite.TestCases.Add(new TestCase(new object[] { i }, i < 2000 ? "" : "large"));
			}
			for (int i = 0; i < 980; i++) {
				int n = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
				testSuite.TestCases.Add(new TestCase(
					new object[] { n },
					n < 1000 ? "small" : (n >= 2000 ? "large" : "")));
			}
			return testSuite;
		}

		protected override void GetCodeTemplate(CodeTemplateBuilder builder) {
			base.GetCodeTemplate(builder);
			builder.AddParameter(typeof(int), "n")
				.SetParameters();
		}

		protected override void GetInstructionSet(InstructionSetBuilder builder) {
			base.GetInstructionSet(builder);
			builder.AddIntegerDomain()
				.AddBooleanDomain()
				.AddIfStatement()
				.AddForLoopTimesStatement()
				.AddForLoopVariable()
				.AddIntVariable("n")
				.AddStringLiterals("small", "large")
				.AddIntERC(LowerBoundValue, UpperBoundValue);
		}
	}
}
