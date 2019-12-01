using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Util;

namespace PatternGuidedGP.GP.Problems.Advanced {
	class AverageProblem : CodingProblem {
		public override Type ReturnType => typeof(float);

		public int TestCases { get; set; } = 50;
		public int UpperBoundValue { get; set; } = 100;
		public int LowerBoundValue { get; set; } = -100;
		public int MinArrayLength { get; set; } = 3;
		public int MaxArrayLength { get; set; } = 10;

		public override IFitnessCalculator FitnessCalculator => new AbsoluteDistanceFitnessCalculator();

		public AverageProblem(bool initialize = true) : base(initialize) {
		}

		// Test values are generated randomly
		protected override TestSuite GetTestSuite() {
			TestSuite testSuite = new TestSuite();
			for (int i = 0; i < TestCases; i++) {
				var arrayLength = RandomValueGenerator.Instance.GetInt(MinArrayLength, MaxArrayLength + 1) + MinArrayLength;
				var array = new float[arrayLength];
				float sum = 0.0f;
				for (int j = 0; j < arrayLength; j++) {
					float value = RandomValueGenerator.Instance.GetFloat(UpperBoundValue - LowerBoundValue) + LowerBoundValue;
					array[j] = value;
					sum += value;
				}
				var average = sum / arrayLength;
				var testCase = new TestCase(new object[] { array, arrayLength }, average);
				testSuite.TestCases.Add(testCase);
			}
			return testSuite;
		}

		protected override void GetCodeTemplate(CodeTemplateBuilder builder) {
			builder.AddParameter(typeof(float), "values", true)
				.AddParameter(typeof(int), "length", false)
				.SetParameters();
		}

		protected override void GetInstructionSet(InstructionSetBuilder builder) {
			builder.AddFloatDomain()
				.AddIntegerDomain()
				.AddBooleanDomain()
				.AddIfStatement()
				.AddForLoopTimesStatement()
				.AddForLoopVariable()
				.AddFloatTargetVariable()
				.AddFloatVariable("values", true)
				.AddIntVariable("length");
		}
	}
}
