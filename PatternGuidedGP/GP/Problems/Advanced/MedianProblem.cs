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

		public int TestCases { get; set; } = 50;
		public int UpperBoundValue { get; set; } = 100;
		public int LowerBoundValue { get; set; } = -100;
		public int MinArrayLength { get; set; } = 3;
		public int MaxArrayLength { get; set; } = 3;

		public override IFitnessCalculator FitnessCalculator => new AbsoluteDistanceFitnessCalculator();

		public MedianProblem(bool initialize = true) : base(initialize) {
		}

		// Test values are generated randomly
		protected override TestSuite GetTestSuite() {
			TestSuite testSuite = new TestSuite();
			int[] arrayLengthOptions = GetArrayLengthOptions();
			for (int i = 0; i < TestCases; i++) {
				var arrayLength = arrayLengthOptions[RandomValueGenerator.Instance.GetInt(arrayLengthOptions.Length)];
				var array = new int[arrayLength];
				for (int j = 0; j < arrayLength; j++) {
					int value = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
					array[j] = value;
				}
				int median = GetMedian(array);
				var testCase = new TestCase(new object[] { array, arrayLength }, median);
				testSuite.TestCases.Add(testCase);
			}
			return testSuite;
		}

		private int GetMedian(int[] array) {
			// do not sort the original array
			int[] copy = new int[array.Length];
			Array.Copy(array, copy, 0);
			Array.Sort(copy);
			return copy[copy.Length / 2];
		}

		protected override void GetCodeTemplate(CodeTemplateBuilder builder) {
			builder.AddParameter(typeof(int), "values", true)
				.AddParameter(typeof(int), "length", false)
				.SetParameters();
		}

		protected override void GetInstructionSet(InstructionSetBuilder builder) {
			builder.AddIntegerDomain()
				.AddBooleanDomain()
				.AddIfStatement()
				.AddForLoopTimesStatement()
				.AddForLoopVariable()
				.AddIntTargetVariable()
				.AddIntVariable("values", true)
				.AddIntVariable("length");
		}

		private int[] GetArrayLengthOptions() {
			return Enumerable.Range(MinArrayLength, MaxArrayLength - MinArrayLength + 1)
				.Where(i => i % 2 == 1).ToArray();
		}
	}
}
