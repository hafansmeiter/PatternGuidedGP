﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP.Problems.Simple;
using PatternGuidedGP.GP.Tests;

namespace PatternGuidedGP.GP.Problems.Simple {
	class IsOrderedProblem : SimpleCodingProblem {
		public override Type ReturnType => typeof(bool);
		public override Type ParameterType => typeof(int);

		public IsOrderedProblem(int parameterCount) : base(parameterCount) {
		}

		protected override TestSuite GetTestSuite() {
			return new IntTestSuiteGenerator().Create(ParameterCount, parameters => {
				for (int i = 0; i < parameters.Length - 1; i++) {
					if ((int) parameters[i] > (int) parameters[i + 1]) {
						return false;
					}
				}
				return true;
			});
		}

		public override IEnumerable<SyntaxTree> GetOptimalSolutions() {
			IList<SyntaxTree> trees = new List<SyntaxTree>();

			var a = new IntIdentifierExpression("a");
			var b = new IntIdentifierExpression("d");
			var c = new IntIdentifierExpression("c");
			var d = new IntIdentifierExpression("d");
			var ret = new BoolIdentifierExpression("ret");
			var trueVal = new BoolTrueExpression();
			var falseVal = new BoolFalseExpression();

			/**
			 * Solution 1 (for 3 parameters):
			 * if (a <= b && b <= c) {
			 *   ret = true;
			 * } else {
			 *   ret = false;
			 * }
			 */
			if (ParameterCount == 3) {
				trees.Add(new SyntaxTree(new IfStatement() {
					Children = {
						new BoolAndExpression() {
							Children = {
								new BoolGreaterEqualIntExpression() {
									Children = {
										a, b
									}
								},
								new BoolGreaterEqualIntExpression() {
									Children = {
										b, c
									}
								}
							}
						},
						new BoolAssignmentStatement() {
							Children = {
								ret, trueVal
							}
						},
						new BoolAssignmentStatement() {
							Children = {
								ret, falseVal
							}
						}
					}
				}));
			}
			else if (ParameterCount == 4) {
				trees.Add(new SyntaxTree(new IfStatement() {
					Children = {
						new BoolAndExpression() {
							Children = {
								new BoolAndExpression() {
									Children = {
										new BoolGreaterEqualIntExpression() {
											Children = {
												a, b
											}
										},
										new BoolGreaterEqualIntExpression() {
											Children = {
												b, c
											}
										}
									}
								},
								new BoolGreaterEqualIntExpression() {
									Children = {
										c, d
									}
								}
							}
						},
						new BoolAssignmentStatement() {
							Children = {
								ret, trueVal
							}
						},
						new BoolAssignmentStatement() {
							Children = {
								ret, falseVal
							}
						}
					}
				}));
			}

			/**
			 * Solution 2 (for 3 parameters):
			 * if (a > b || b > c) {
			 *   ret = false;
			 * } else {
			 *   ret = true;
			 * }
			 */
			if (ParameterCount == 3) {
				trees.Add(new SyntaxTree(new IfStatement() {
					Children = {
						new BoolOrExpression() {
							Children = {
								new BoolGreaterThanIntExpression() {
									Children = {
										a, b
									}
								},
								new BoolGreaterThanIntExpression() {
									Children = {
										b, c
									}
								}
							}
						},
						new BoolAssignmentStatement() {
							Children = {
								ret, falseVal
							}
						},
						new BoolAssignmentStatement() {
							Children = {
								ret, trueVal
							}
						}
					}
				}));
			}
			else if (ParameterCount == 4) {
				trees.Add(new SyntaxTree(new IfStatement() {
					Children = {
						new BoolOrExpression() {
							Children = {
								new BoolOrExpression() {
									Children = {
										new BoolGreaterThanIntExpression() {
											Children = {
												a, b
											}
										},
										new BoolGreaterThanIntExpression() {
											Children = {
												b, c
											}
										}
									}
								},
								new BoolGreaterThanIntExpression() {
									Children = {
										c, d
									}
								}
							}
						},
						new BoolAssignmentStatement() {
							Children = {
								ret, falseVal
							}
						},
						new BoolAssignmentStatement() {
							Children = {
								ret, trueVal
							}
						}
					}
				}));
			}
			return trees;
		}
	}
}
