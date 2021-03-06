﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Util;

namespace PatternGuidedGP.GP.Problems.Advanced {
	class MedianProblem : CodingProblem {
		public override Type ReturnType => typeof(int);

		public int UpperBoundValue { get; set; } = 10;
		public int LowerBoundValue { get; set; } = 0;

		public override IFitnessCalculator FitnessCalculator => new EqualityFitnessCalculator();

		public MedianProblem(bool initialize = true) : base(3, initialize) {
		}

		// Test values are generated randomly
		protected override TestSuite GetTestSuite() {
			TestSuite testSuite = new TestSuite();
			// triplet of integers, all equal
			for (int i = 0; i < 5; i++) {
				int value = RandomValueGenerator.Instance.GetInt(LowerBoundValue, UpperBoundValue);
				var testCase = new TestCase(new object[] { value, value, value }, value);
				testSuite.TestCases.Add(testCase);
			}

			// triplet of integers, two of three equal
			for (int i = 0; i < 15; i++) {
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
			for (int i = 0; i < 30; i++) {
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
			Array.Sort(array);
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

		protected override IEnumerable<SyntaxTree> CreateOptimalSolutions() {
			IList<SyntaxTree> trees = new List<SyntaxTree>();

			var a = new IntIdentifierExpression("a");
			var b = new IntIdentifierExpression("b");
			var c = new IntIdentifierExpression("c");
			var ret = new IntIdentifierExpression("ret");

			/**
			 * Solution (for 3 parameters):
			 * if (a >= b) {
			 *   if (c >= a) {
			 *     ret = a;
			 *   } else if (c >= b) {
			 *     ret = c;
			 *   } else {
			 *     ret = b;
			 *   }
			 * } else {
			 *   if (c >= b) {
			 *     ret = b;
			 *   } else if (c >= a) {
			 *     ret = c;
			 *   } else {
			 *     ret = a;
			 *   }
			 * }
			 */
			trees.Add(new SyntaxTree(new IfStatement() {
				Children = {
						new BoolGreaterEqualIntExpression() {
							Children = {
								a, b
							}
						},
						new IfStatement() {
							Children = {
								new BoolGreaterEqualIntExpression() {
									Children = {
										c, a
									}
								},
								new IntAssignmentStatement() {
									Children = {
										ret, a
									}
								},
								new IfStatement() {
									Children = {
										new BoolGreaterEqualIntExpression() {
											Children = {
												c, b
											}
										},
										new IntAssignmentStatement() {
											Children = {
												ret, c
											}
										},
										new IntAssignmentStatement() {
											Children = {
												ret, b
											}
										}
									}
								}
							}
						},
						new IfStatement() {
							Children = {
								new BoolGreaterEqualIntExpression() {
									Children = {
										c, b
									}
								},
								new IntAssignmentStatement() {
									Children = {
										ret, b
									}
								},
								new IfStatement() {
									Children = {
										new BoolGreaterEqualIntExpression() {
											Children = {
												c, a
											}
										},
										new IntAssignmentStatement() {
											Children = {
												ret, c
											}
										},
										new IntAssignmentStatement() {
											Children = {
												ret, a
											}
										}
									}
								}
							}
						}
					}
			}));
			return trees;
		}
	}
}
