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
	class CountOddsProblem : CodingProblem {
		public override IFitnessCalculator FitnessCalculator => new EqualityFitnessCalculator();
		public override Type ReturnType => typeof(int);

		public int UpperBoundValue { get; set; } = 1000;
		public int LowerBoundValue { get; set; } = -1000;
		public int MinArrayLength { get; set; } = 3;
		public int MaxArrayLength { get; set; } = 5;

		protected override TestSuite GetTestSuite() {
			TestSuite testSuite = new TestSuite();

			// fixed specific arrays
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { }, 0 }, 0));
			for (int i = -10; i <= 10; i++) {
				testSuite.TestCases.Add(new TestCase(new object[] { new int[] { i }, 1 }, i % 2 != 0 ? 1 : 0));
			}
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { -947 }, 1 }, 1));
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { -450 }, 1 }, 0));
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { 303 }, 1 }, 1));
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { 886 }, 1 }, 0));
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { 0, 0 }, 2 }, 0));
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { 0, 1 }, 2 }, 1));
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { 7, 1 }, 2 }, 2));
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { -9, -1 }, 2 }, 2));
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { -11, -40 }, 2 }, 1));
			testSuite.TestCases.Add(new TestCase(new object[] { new int[] { 944, 77 }, 2 }, 1));

			// arrays with odd values only
			for (int i = 0; i < 9; i++) {
				int arrayLength = RandomValueGenerator.Instance.GetInt(MinArrayLength, MaxArrayLength);
				var array = new int[arrayLength];
				for (int j = 0; j < arrayLength; j++) {
					int value = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
					array[j] = IsOdd(value) ? value : value + 1;
				}
				testSuite.TestCases.Add(new TestCase(new object[] { array, arrayLength }, arrayLength));
			}

			// arrays with even values only
			for (int i = 0; i < 9; i++) {
				int arrayLength = RandomValueGenerator.Instance.GetInt(MinArrayLength, MaxArrayLength);
				var array = new int[arrayLength];
				for (int j = 0; j < arrayLength; j++) {
					int value = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
					array[j] = IsOdd(value) ? value + 1 : value;
				}
				testSuite.TestCases.Add(new TestCase(new object[] { array, arrayLength }, 0));
			}

			// arrays with random values
			for (int i = 0; i < 50; i++) {
				int arrayLength = RandomValueGenerator.Instance.GetInt(MinArrayLength, MaxArrayLength);
				var array = new int[arrayLength];
				int odds = 0;
				for (int j = 0; j < arrayLength; j++) {
					int value = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
					array[j] = value;
					if (IsOdd(value)) {
						odds++;
					}
				}
				testSuite.TestCases.Add(new TestCase(new object[] { array, arrayLength }, odds));
			}

			return testSuite;
		}

		private bool IsOdd(int i) {
			return i % 2 != 0;
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
				//.AddIntegerLiterals(1, 2)
				.AddIntRandomLiteral(1, 10);
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
			var zero = new IntLiteralExpression(0);
			var one = new IntLiteralExpression(1);
			var two = new IntLiteralExpression(2);

			/**
			 * Solution:
			 * for (int i = 0; i < n; i++) {
			 *   if (values[i] % 2 == 0) {
			 *     ret = ret + 1;
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
									new IntModuloExpression() {
										Children = {
											values,
											two
										}
									},
									zero
								}
							},
							new IntAssignmentStatement() {
								Children = {
									ret,
									new IntAdditionExpression() {
										Children = {
											ret, one
										}
									}
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
