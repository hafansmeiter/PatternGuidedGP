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
	class MedianProblem : CodingProblem {
		public override Type ReturnType => typeof(int);

		public int UpperBoundValue { get; set; } = 100;
		public int LowerBoundValue { get; set; } = -100;

		public override IFitnessCalculator FitnessCalculator => new EqualityFitnessCalculator();

		public MedianProblem(bool initialize = true) : base(3, true) {
		}

		// Test values are generated randomly
		protected override TestSuite GetTestSuite() {
			TestSuite testSuite = new TestSuite();
			// triplet of integers, all equal
			for (int i = 0; i < 10; i++) {
				int value = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
				var testCase = new TestCase(new object[] { value, value, value}, value);
				testSuite.TestCases.Add(testCase);
			}

			// triplet of integers, two of three equal
			for (int i = 0; i < 30; i++) {
				var array = new int[ParameterCount];
				int value = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
				for (int j = 0; j < ParameterCount; j++) {
					array[j] = value;
				}
				int otherValue = value;
				while (otherValue == value) {
					otherValue = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
				}
				int changeAtIndex = RandomValueGenerator.Instance.GetInt(ParameterCount);
				array[changeAtIndex] = otherValue;
				var testCase = new TestCase(new object[] { array[0], array[1], array[2] }, value);
				testSuite.TestCases.Add(testCase);
			}

			// triplet of any integers, no values equal
			for (int i = 0; i < 60; i++) {
				int value1 = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
				int value2 = value1;
				while (value1 == value2) {
					value2 = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
				}
				int value3 = value1;
				while (value1 == value3 || value2 == value3) {
					value3 = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
				}
				int median = GetMedian(value1, value2, value3);
				var testCase = new TestCase(new object[] { value1, value2, value3 }, median);
				testSuite.TestCases.Add(testCase);
			}
			return testSuite;
		}

		private int GetMedian(params int[] array) {
			return array[array.Length / 2];
		}

		protected override void GetCodeTemplate(CodeTemplateBuilder builder) {
			base.GetCodeTemplate(builder);
			builder.UseParameterType(typeof(int), ParameterCount);
		}

		protected override void GetInstructionSet(InstructionSetBuilder builder) {
			base.GetInstructionSet(builder);
			builder.AddIntegerDomain()
				.AddBooleanDomain()
				.AddIfStatement()
				.AddForLoopTimesStatement()
				.AddForLoopVariable()
				.AddIntVariable("a")
				.AddIntVariable("b")
				.AddIntVariable("c")
				.AddIntRandomLiteral(-100, 100);
		}
	}
}
