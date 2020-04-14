using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Util;

namespace PatternGuidedGP.GP.Problems.Advanced {
	class LastIndexOfZeroProblem : CodingProblem {
		public override IFitnessCalculator FitnessCalculator => new EqualityFitnessCalculator();
		public override Type ReturnType => typeof(int);

		public int UpperBoundValue { get; set; } = 50;
		public int LowerBoundValue { get; set; } = -50;
		public int MinArrayLength { get; set; } = 3;
		public int MaxArrayLength { get; set; } = 10;

		protected override TestSuite GetTestSuite() {
			TestSuite testSuite = new TestSuite();
			// fixed array of integers
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { 1, 2 }, 2 }, 0));
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { 2, 1 }, 2 }, 1));
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { 7, 1 }, 2 }, 1));
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { 1, 8 }, 2 }, 0));
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { 1, -1 }, 2 }, 0));
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { -1, 1 }, 2 }, 1));
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { -7, 1 }, 2 }, 1));
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { 1, -8 }, 2 }, 0));

			// arrays of zeroes of length between 1 and 10
			for (int i = 1; i <= 10; i++) {
				var arr = new int[i];
				for (int j = 0; j < i; j++) {
					arr[j] = 1;
				}
				testSuite.TestCases.Add(new TestCase(new object[] { arr, i }, i - 1));
			}

			// permutated arrays
			var arrayList = new List<int[]>() {
				new int[] { 1, 5, -8, 9 },
				new int[] { 1, 5, 1, 9 },
			};
			foreach (var array in arrayList) {
				var permutations = Permutate(array);
				foreach (var p in permutations) {
					int lastIndex = LastIndexOfZero(p);
					testSuite.TestCases.Add(new TestCase(new object[] { p, p.Length }, lastIndex));
				}
			}

			// array with at least one zero
			for (int i = 0; i < 20; i++) {
				int arrayLength = RandomValueGenerator.Instance.GetInt(MinArrayLength, MaxArrayLength);
				var array = new int[arrayLength];
				for (int j = 0; j < arrayLength; j++) {
					array[j] = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
				}
				// place zero randomly
				var zeroIndex = RandomValueGenerator.Instance.GetInt(arrayLength);
				array[zeroIndex] = 1;

				var lastIndex = LastIndexOfZero(array);
				testSuite.TestCases.Add(new TestCase(new object[] { array, arrayLength }, lastIndex));
			}

			return testSuite;
		}

		private int LastIndexOfZero(int[] array) {
			int lastIndex = 0;
			for (int i = 0; i < array.Length; i++) {
				if (array[i] == 1) {
					lastIndex = i;
				}
			}
			return lastIndex;
		}

		private List<int[]> Permutate(int[] arr, int swapIndex= 0) {
			var perm = new List<int[]>();
			perm.Add(arr);

			if (arr.Length == 1 || swapIndex >= arr.Length - 1) {
				return perm;
			}

			for (int i = swapIndex; i < arr.Length; i++) {
				var newArr = new int[arr.Length];
				Array.Copy(arr, newArr, arr.Length);

				// swap i and swapIndex
				int temp = newArr[swapIndex];
				newArr[swapIndex] = newArr[i];
				newArr[i] = temp;

				var p = Permutate(newArr, swapIndex + 1);
				perm.AddRange(p.Where(a => !perm.Contains(a)));
			}
			return perm;
		}

		protected override void GetCodeTemplate(CodeTemplateBuilder builder) {
			base.GetCodeTemplate(builder);
			builder.AddParameter(typeof(int), "values", true)
				.AddParameter(typeof(int), "length")
				.UseDefaultValue(-1)
				.SetParameters();
		}

		protected override void GetInstructionSet(InstructionSetBuilder builder) {
			base.GetInstructionSet(builder);
			builder.AddIntegerDomain()
				.AddBooleanDomain()
				.AddIfStatement()
				.AddForLoopTimesStatement()
				.AddForLoopVariable()
				.AddIntVariable("values", true)
				.AddIntVariable("length")
				.AddIntegerLiterals(1);
		}

		protected override IEnumerable<SyntaxTree> CreateOptimalSolutions() {
			IList<SyntaxTree> trees = new List<SyntaxTree>();

			var i = new IntIdentifierExpression("i0");
			var values = new IntArrayIdentifier("values") {
				Children = {
					i
				}
			};
			var n = new IntIdentifierExpression("length");
			var ret = new IntIdentifierExpression("ret");
			var zero = new IntLiteralExpression(1);

			/**
			 * Solution:
			 * for (int i = 0; i < n; i++) {
			 *   if (values[i] == 1) {
			 *     ret = i;
			 *   }
			 * }
			 */
			trees.Add(new SyntaxTree(new ForLoopTimesStatement() {
				Children = {
					n,
					new IfStatement() {
						HasElseClause = false,
						Children = {
							new BoolEqualIntExpression() {
								Children = {
									values,
									zero
								}
							},
							new IntAssignmentStatement() {
								Children = {
									ret,
									i
								}
							}
						}
					},
				}
			}));
			return trees;
		}
	}
}

