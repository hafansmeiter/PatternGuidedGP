using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP.Problems.Simple;
using PatternGuidedGP.GP.Tests;

namespace PatternGuidedGP.GP.Problems.Simple {
	class MaximumProblem : SimpleCodingProblem {
		public override Type ReturnType => typeof(int);
		public override Type ParameterType => typeof(int);

		public MaximumProblem(int parameterCount) : base(parameterCount) {
		}

		protected override TestSuite GetTestSuite() {
			return new IntTestSuiteGenerator().Create(ParameterCount, parameters => {
				int max = (int) parameters[0];
				for (int i = 1; i < parameters.Length; i++) {
					if ((int) parameters[i] > max) {
						max = (int)parameters[i];
					}
				}
				return max;
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
			var one = new IntLiteralExpression(1);
			var two = new IntLiteralExpression(2);
			var three = new IntLiteralExpression(3);
			var four = new IntLiteralExpression(4);

			/**
			 * Solution 1 (for 3 parameters):
			 * if (a > b) {
			 *   if (a > c) {
			 *     ret = a;
			 *   } else {
			 *     ret = c;
			 *   }
			 * } else {
			 *   if (b > c) {
			 *     ret = b;
			 *   } else {
			 *     ret = c;
			 *   }
			 * }
			 */

			if (ParameterCount == 3) {
				trees.Add(new SyntaxTree(new IfStatement() {
					Children = {
						new BoolGreaterThanIntExpression() {
							Children = {
								a, b
							}
						},
						new IfStatement() {
							Children = {
								new BoolGreaterThanIntExpression() {
									Children = {
										a, c
									}
								},
								new IntAssignmentStatement() {
									Children = {
										ret, a
									}
								},
								new IntAssignmentStatement() {
									Children = {
										ret, c
									}
								}
							}
						},
						new IfStatement() {
							Children = {
								new BoolGreaterThanIntExpression() {
									Children = {
										b, c
									}
								},
								new IntAssignmentStatement() {
									Children = {
										ret, b
									}
								},
								new IntAssignmentStatement() {
									Children = {
										ret, c
									}
								}
							}
						}
					}
				}));
			}

			/**
			 * Solution 1 (for 4 parameters):
			 * if (a > b) {
			 *   if (a > c) {
			 *     if (a > d) {
			 *       ret = a;
			 *     } else {
			 *       ret = d;
			 *     }
			 *   } else {
			 *     if (c > d) {
			 *       ret = c;
			 *     } else {
			 *       ret = d;
			 *     }
			 *   }
			 * } else {
			 *   if (b > c) {
			 *     if (b > d) {
			 *       ret = b;
			 *     } else {
			 *       ret = d;
			 *     }
			 *   } else {
			 *     if (c > d) {
			 *       ret = c;
			 *     } else {
			 *       ret = d;
			 *     }
			 *   }
			 * }
			 */

			trees.Add(new SyntaxTree(new IfStatement() {
				Children = {
					new BoolGreaterThanIntExpression() {
						Children = {
							a, b
						}
					},
					new IfStatement() {
						Children = {
							new BoolGreaterThanIntExpression() {
								Children = {
									a, c
								}
							},
							new IfStatement() {
								Children = {
									new BoolGreaterThanIntExpression() {
										Children = {
											a, d
										}
									},
									new IntAssignmentStatement() {
										Children = {
											ret, a
										}
									},
									new IntAssignmentStatement() {
										Children = {
											ret, d
										}
									}
								}
							},
							new IfStatement() {
								Children = {
									new BoolGreaterThanIntExpression() {
										Children = {
											c, d
										}
									},
									new IntAssignmentStatement() {
										Children = {
											ret, c
										}
									},
									new IntAssignmentStatement() {
										Children = {
											ret, d
										}
									}
								}
							},
						}
					},
					new IfStatement() {
						Children = {
							new BoolGreaterThanIntExpression() {
								Children = {
									b, c
								}
							},
							new IfStatement() {
								Children = {
									new BoolGreaterThanIntExpression() {
										Children = {
											b, d
										}
									},
									new IntAssignmentStatement() {
										Children = {
											ret, b
										}
									},
									new IntAssignmentStatement() {
										Children = {
											ret, d
										}
									}
								}
							},
							new IfStatement() {
								Children = {
									new BoolGreaterThanIntExpression() {
										Children = {
											c, d
										}
									},
									new IntAssignmentStatement() {
										Children = {
											ret, c
										}
									},
									new IntAssignmentStatement() {
										Children = {
											ret, d
										}
									}
								}
							},
						}
					}
				}
			}));

			return trees;
		}
	}
}
