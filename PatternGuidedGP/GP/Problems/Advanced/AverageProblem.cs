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

		public int UpperBoundValue { get; set; } = 1000;
		public int LowerBoundValue { get; set; } = -1000;
		public int MinArrayLength { get; set; } = 3;
		public int MaxArrayLength { get; set; } = 10;

		public override IFitnessCalculator FitnessCalculator => new AbsoluteDistanceFitnessCalculator();

		public AverageProblem(bool initialize = true) : base(initialize) {
		}

		protected override TestSuite GetTestSuite() {
			TestSuite testSuite = new TestSuite();

			// arrays of specific fixed float values
			var array1 = new float[1];
			array1[0] = 0.0f;
			testSuite.TestCases.Add(new TestCase(new object[] { array1, 1 }, 0.0f));

			var array2 = new float[1];
			array2[0] = 100.0f;
			testSuite.TestCases.Add(new TestCase(new object[] { array2, 1 }, 100.0f));

			var array3 = new float[1];
			array3[0] = -100.0f;
			testSuite.TestCases.Add(new TestCase(new object[] { array3, 1 }, -100.0f));

			var array4 = new float[2];
			array4[0] = 2.0f;
			array4[1] = 129.0f;
			testSuite.TestCases.Add(new TestCase(new object[] { array4, 2 }, (array4[0] + array4[1]) / 2.0f));

			var array5 = new float[2];
			array5[0] = 0.12345f;
			array5[1] = -4.678f;
			testSuite.TestCases.Add(new TestCase(new object[] { array5, 2 }, (array5[0] + array5[1]) / 2.0f));

			var array6 = new float[2];
			array6[0] = 999.99f;
			array6[1] = 74.113f;
			testSuite.TestCases.Add(new TestCase(new object[] { array6, 2 }, (array6[0] + array6[1]) / 2.0f));

			// arrays of length 50 of random float values
			for (int i = 0; i < 4; i++) {
				var array = new float[50];
				var sum = 0.0f;
				for (int j = 0; j < array.Length; j++) {
					array[j] = RandomValueGenerator.Instance.GetFloat(UpperBoundValue - LowerBoundValue) + LowerBoundValue;
					sum += array[j];
				}
				var average = sum / 50.0f;
				testSuite.TestCases.Add(new TestCase(new object[] { array, 50 }, average));
			}

			// arrays of random float values
			for (int i = 0; i < 90; i++) {
				var arrayLength = RandomValueGenerator.Instance.GetInt(MinArrayLength, MaxArrayLength);
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
			base.GetCodeTemplate(builder);
			builder.AddParameter(typeof(float), "values", true)
				.AddParameter(typeof(int), "length", false)
				.SetParameters();
		}

		protected override void GetInstructionSet(InstructionSetBuilder builder) {
			base.GetInstructionSet(builder);
			builder.AddFloatDomain()
				.AddIntegerDomain()
				.AddBooleanDomain()
				.AddIfStatement()
				.AddForLoopTimesStatement()
				.AddForLoopVariable()
				.AddFloatVariable("values", true)
				.AddIntVariable("length");
		}
	}
}
