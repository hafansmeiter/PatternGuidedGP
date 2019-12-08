using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Util;

namespace PatternGuidedGP.GP.Problems.Advanced {
	class MirrorImageProblem : CodingProblem {
		public override IFitnessCalculator FitnessCalculator => new EqualityFitnessCalculator();
		public override Type ReturnType => typeof(bool);

		public int UpperBoundValue { get; set; } = 50;
		public int LowerBoundValue { get; set; } = -50;
		public int MinArrayLength { get; set; } = 3;
		public int MaxArrayLength { get; set; } = 10;

		protected override TestSuite GetTestSuite() {
			TestSuite testSuite = new TestSuite();

			// add fixed specific arrays
			var arrays = new List<Tuple<int[], int[]>>();
			arrays.Add(Tuple.Create(new int[] {   }, new int[] {   }));
			arrays.Add(Tuple.Create(new int[] { 1 }, new int[] { 1 }));
			arrays.Add(Tuple.Create(new int[] { 0 }, new int[] { 1 }));
			arrays.Add(Tuple.Create(new int[] { 1 }, new int[] { 0 }));
			arrays.Add(Tuple.Create(new int[] { -44 }, new int[] { 16 }));
			arrays.Add(Tuple.Create(new int[] { -13 }, new int[] { -12 }));
			arrays.Add(Tuple.Create(new int[] { 2, 1 }, new int[] { 1, 2 }));
			arrays.Add(Tuple.Create(new int[] { 0, 1 }, new int[] { 1, 1 }));
			arrays.Add(Tuple.Create(new int[] { 0, 7 }, new int[] { 7, 0 }));
			arrays.Add(Tuple.Create(new int[] { 5, 8 }, new int[] { 5, 8 }));
			arrays.Add(Tuple.Create(new int[] { 34, 12 }, new int[] { 34, 12 }));
			arrays.Add(Tuple.Create(new int[] { 456, 456 }, new int[] { 456, 456 }));
			arrays.Add(Tuple.Create(new int[] { 40, 831 }, new int[] { -431, -680 }));
			arrays.Add(Tuple.Create(new int[] { 1, 2, 1 }, new int[] { 1, 2, 1 }));
			arrays.Add(Tuple.Create(new int[] { 1, 2, 3, 4, 5, 4, 3, 2, 1 }, new int[] { 1, 2, 3, 4, 5, 4, 3, 2, 1 }));
			arrays.Add(Tuple.Create(new int[] { 45, 99, 0, 12, 44, 7, 7, 44, 12, 0, 99, 45 }, new int[] { 45, 99, 0, 12, 44, 7, 7, 44, 12, 0, 99, 45 }));
			arrays.Add(Tuple.Create(new int[] { 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 }, 
									new int[] { 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 }));
			arrays.Add(Tuple.Create(new int[] { 33, 45, -941 }, new int[] { 33, 45, -941 }));
			arrays.Add(Tuple.Create(new int[] { 33, -941, 45 }, new int[] { 33, 45, -941 }));
			arrays.Add(Tuple.Create(new int[] { 45, 33, -941 }, new int[] { 33, 45, -941 }));
			arrays.Add(Tuple.Create(new int[] { 45, -941, 33 }, new int[] { 33, 45, -941 }));
			arrays.Add(Tuple.Create(new int[] { -941, 33, 45 }, new int[] { 33, 45, -941 }));
			arrays.Add(Tuple.Create(new int[] { -941, 45, 33 }, new int[] { 33, 45, -941 }));

			foreach (var tuple in arrays) {
				testSuite.TestCases.Add(new TestCase(new object[] { tuple.Item1, tuple.Item2, tuple.Item1.Length }, IsMirror(tuple.Item1, tuple.Item2)));
			}

			// add mirrored arrays
			for (int i = 0; i < 37; i++) {
				int arrayLength = RandomValueGenerator.Instance.GetInt(MinArrayLength, MaxArrayLength + 1);
				var array = new int[arrayLength];
				for (int j = 0; j < arrayLength; j++) {
					array[j] = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
				}
				testSuite.TestCases.Add(new TestCase(new object[] { array, GetMirror(array), arrayLength }, true));
			}

			// add equal arrays
			for (int i = 0; i < 10; i++) {
				int arrayLength = RandomValueGenerator.Instance.GetInt(MinArrayLength, MaxArrayLength + 1);
				var array = new int[arrayLength];
				for (int j = 0; j < arrayLength; j++) {
					array[j] = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
				}
				testSuite.TestCases.Add(new TestCase(new object[] { array, array, arrayLength }, false));
			}

			// add close-to-mirrored arrays
			for (int i = 0; i < 20; i++) {
				int arrayLength = RandomValueGenerator.Instance.GetInt(MinArrayLength, MaxArrayLength + 1);
				var array = new int[arrayLength];
				for (int j = 0; j < arrayLength; j++) {
					array[j] = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
				}
				var mirrored = GetMirror(array);
				var changeNumbers = RandomValueGenerator.Instance.GetInt(2) + 1;
				for (int j = 0; j < changeNumbers; j++) {
					var changeIndex = RandomValueGenerator.Instance.GetInt(arrayLength);
					var changeValue = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
					while (changeValue == array[changeIndex]) {
						changeValue = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
					}
					array[changeIndex] = changeValue;
				}
				testSuite.TestCases.Add(new TestCase(new object[] { array, mirrored, arrayLength }, IsMirror(array, mirrored)));
			}

			// add random arrays
			for (int i = 0; i < 10; i++) {
				int arrayLength = RandomValueGenerator.Instance.GetInt(MinArrayLength, MaxArrayLength + 1);
				var array = new int[arrayLength];
				for (int j = 0; j < arrayLength; j++) {
					array[j] = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
				}
				var mirrored = new int[arrayLength];
				for (int j = 0; j < arrayLength; j++) {
					mirrored[j] = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
				}
				testSuite.TestCases.Add(new TestCase(new object[] { array, mirrored, arrayLength }, IsMirror(array, mirrored)));
			}

			return testSuite;
		}

		private int[] GetMirror(int [] array) {
			return array.Reverse().ToArray();
		}

		private bool IsMirror(int[] item1, int[] item2) {
			if (item1.Length != item2.Length) {
				return false;
			}
			for (int i = 0; i < item1.Length; i++) {
				if (item1[i] != item1[item1.Length - i - 1]) {
					return false;
				}
			}
			return true;
		}

		protected override void GetCodeTemplate(CodeTemplateBuilder builder) {
			base.GetCodeTemplate(builder);
			builder.AddParameter(typeof(int), "a", true)
				.AddParameter(typeof(int), "b", true)
				.AddParameter(typeof(int), "length", false)
				.SetParameters();
		}

		protected override void GetInstructionSet(InstructionSetBuilder builder) {
			base.GetInstructionSet(builder);
			builder.AddIntegerDomain()
				.AddBooleanDomain()
				.AddIfStatement()
				.AddForLoopTimesStatement()
				.AddForLoopVariable()
				.AddIntVariable("a", true)
				.AddIntVariable("b", true)
				.AddIntVariable("length");
		}
	}
}
