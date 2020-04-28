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
	class SmallOrLargeProblem : CodingProblem {
		public override IFitnessCalculator FitnessCalculator => new EqualityFitnessCalculator();
		public override Type ReturnType => typeof(int); // small = 1, large = 3, in between = 2

		public int UpperBoundValue { get; set; } = 10000;
		public int LowerBoundValue { get; set; } = -10000;

		protected override TestSuite GetTestSuite() {
			TestSuite testSuite = new TestSuite();
			for (int i = 980; i < 1020; i++) {
				testSuite.TestCases.Add(new TestCase(new object[] { i }, i < 1000 ? 1 : 2));
			}
			for (int i = 1980; i < 2020; i++) {
				testSuite.TestCases.Add(new TestCase(new object[] { i }, i < 2000 ? 2 : 3));
			}
			for (int i = 0; i < 50; i++) {
				int n = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
				testSuite.TestCases.Add(new TestCase(
					new object[] { n },
					n < 1000 ? 1 : (n >= 2000 ? 3 : 2)));
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
				.AddIntegerLiterals(1, 2, 3, 1000, 2000)
				.AddIntRandomLiteral(LowerBoundValue, UpperBoundValue);
		}

		protected override IEnumerable<SyntaxTree> CreateOptimalSolutions() {
			IList<SyntaxTree> trees = new List<SyntaxTree>();

			var n = new IntIdentifierExpression("n");
			var ret = new IntIdentifierExpression("ret");
			var one = new IntLiteralExpression(1);
			var two = new IntLiteralExpression(2);
			var three = new IntLiteralExpression(3);
			var thousand = new IntLiteralExpression(1000);
			var twoThousand = new IntLiteralExpression(2000);

			/**
			 * Solution 1:
			 * if (n < 1000) {
			 *   ret = 1;
			 * } else {
			 *   if (n < 2000) {
			 *     ret = 2;
			 *   } else {
			 *     ret = 3;
			 *   }
			 * }
			 */
			trees.Add(new SyntaxTree(new IfStatement() {
				Children = {
					new BoolLessThanIntExpression() {
						Children = {
							n, thousand
						}
					},
					new IntAssignmentStatement() {
						Children = {
							ret, one
						}
					},
					new IfStatement() {
						Children = {
							new BoolLessThanIntExpression() {
								Children = {
									n, twoThousand
								}
							},
							new IntAssignmentStatement() {
								Children = {
									ret, two
								}
							},
							new IntAssignmentStatement() {
								Children = {
									ret, three
								}
							}
						}
					}
				}
			}));

			/**
			 * Solution 2:
			 * if (n >= 1000) {
			 *   if (n >= 2000) {
			 *     ret = 3;
			 *   } else {
			 *     ret = 2;
			 *   }
			 * } else {
			 *   ret = 1;
			 * }
			 */
			trees.Add(new SyntaxTree(new IfStatement() {
				Children = {
					new BoolGreaterEqualIntExpression() {
						Children = {
							n, thousand
						}
					},
					new IfStatement() {
						Children = {
							new BoolGreaterEqualIntExpression() {
								Children = {
									n, twoThousand
								}
							},
							new IntAssignmentStatement() {
								Children = {
									ret, three
								}
							},
							new IntAssignmentStatement() {
								Children = {
									ret, two
								}
							}
						}
					},
					new IntAssignmentStatement() {
						Children = {
							ret, one
						}
					}
				}
			}));
			return trees;
		}
	}
}
