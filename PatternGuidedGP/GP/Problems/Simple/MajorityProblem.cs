using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.GP.Problems.Simple;
using PatternGuidedGP.GP.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Problems.Simple {
	class MajorityProblem : SimpleCodingProblem {
		public override Type ReturnType => typeof(bool);
		public override Type ParameterType => typeof(int);

		public MajorityProblem(int n) : base(n) {
		}

		protected override TestSuite GetTestSuite() {
			return new IntTestSuiteGenerator().Create(ParameterCount, parameters => {
				int ones = 0;
				foreach (var par in parameters) {
					if (((int)par) == 1) {
						ones++;
					}
				}
				return ones > ParameterCount / 2;
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
			 * if (((a == 1 && b == 1) || (b == 1 && c == 1)) || (a == 1 && c == 1)) {
			 *   ret = true;
			 * } else {
			 *   ret = false;
			 * }
			 */
			if (ParameterCount == 3) {
				trees.Add(new SyntaxTree(new IfStatement() {
					Children = {
						new BoolOrExpression() {
							Children = {
								new BoolOrExpression() {
									Children = {
										new BoolAndExpression() {
											Children = {
												new BoolEqualIntExpression() {
													Children = {
														a, one
													}
												},
												new BoolEqualIntExpression() {
													Children = {
														b, one
													}
												}
											}
										},
										new BoolAndExpression() {
											Children = {
												new BoolEqualIntExpression() {
													Children = {
														b, one
													}
												},
												new BoolEqualIntExpression() {
													Children = {
														c, one
													}
												}
											}
										}
									}
								},
								new BoolAndExpression() {
									Children = {
										new BoolEqualIntExpression() {
											Children = {
												a, one
											}
										},
										new BoolEqualIntExpression() {
											Children = {
												c, one
											}
										}
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
			 * Solution 1 (for 4 parameters):
			 * if (((a == 1 && b == 1) && c == 1) || (a == 1 && b == 1) && d == 1)) ||
			 *   ((a == 1 && c == 1) && d == 1) || (b == 1 && c == 1) && d == 1))) {
			 *     ret = true;
			 *  else {
			 *    ret = false;
			 *  }
			 */
			if (ParameterCount == 4) {
				trees.Add(new SyntaxTree(new IfStatement() {
					Children = {
						new BoolOrExpression() {
							Children = {
								new BoolOrExpression() {
									Children = {
										new BoolAndExpression() {
											Children = {
												new BoolAndExpression() {
													Children = {
														new BoolEqualIntExpression() {
															Children = {
																a, one
															}
														},
														new BoolEqualIntExpression() {
															Children = {
																b, one
															}
														}
													}
												},
												new BoolEqualIntExpression() {
													Children = {
														c, one
													}
												}
											}
										},
										new BoolAndExpression() {
											Children = {
												new BoolAndExpression() {
													Children = {
														new BoolEqualIntExpression() {
															Children = {
																a, one
															}
														},
														new BoolEqualIntExpression() {
															Children = {
																b, one
															}
														}
													}
												},
												new BoolEqualIntExpression() {
													Children = {
														d, one
													}
												}
											}
										}
									}
								},
								new BoolOrExpression() {
									Children = {
										new BoolAndExpression() {
											Children = {
												new BoolAndExpression() {
													Children = {
														new BoolEqualIntExpression() {
															Children = {
																a, one
															}
														},
														new BoolEqualIntExpression() {
															Children = {
																c, one
															}
														}
													}
												},
												new BoolEqualIntExpression() {
													Children = {
														d, one
													}
												}
											}
										},
										new BoolAndExpression() {
											Children = {
												new BoolAndExpression() {
													Children = {
														new BoolEqualIntExpression() {
															Children = {
																b, one
															}
														},
														new BoolEqualIntExpression() {
															Children = {
																c, one
															}
														}
													}
												},
												new BoolEqualIntExpression() {
													Children = {
														d, one
													}
												}
											}
										}
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
			return trees;
		}
	}
}
