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
	class ContainsFirstProblem : SimpleCodingProblem {
		public override Type ReturnType => typeof(bool);
		public override Type ParameterType => typeof(int);

		public ContainsFirstProblem(int parameterCount) : base(parameterCount) {
		}

		protected override TestSuite GetTestSuite() {
			return new IntTestSuiteGenerator().Create(ParameterCount, parameters => {
				int value = (int) parameters[0];
				for (int i = 1; i < parameters.Length; i++) {
					if (value == (int) parameters[i]) {
						return true;
					}
				}
				return false;
			});
		}

		protected override IEnumerable<SyntaxTree> CreateOptimalSolutions() {
			IList<SyntaxTree> trees = new List<SyntaxTree>();

			var a = new IntIdentifierExpression("a");
			var b = new IntIdentifierExpression("b");
			var c = new IntIdentifierExpression("c");
			var d = new IntIdentifierExpression("d");
			var ret = new BoolIdentifierExpression("ret");
			var trueVal = new BoolTrueExpression();
			var falseVal = new BoolFalseExpression();

			/**
			 * Solution 1 (for 3 parameters):
			 * if (a == b || a == c) {
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
								new BoolEqualIntExpression() {
									Children = {
										a, b
									}
								},
								new BoolEqualIntExpression() {
									Children = {
										a, c
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
						new BoolOrExpression() {
							Children = {
								new BoolOrExpression() {
									Children = {
										new BoolEqualIntExpression() {
											Children = {
												a, b
											}
										},
										new BoolEqualIntExpression() {
											Children = {
												a, c
											}
										}
									}
								},
								new BoolEqualIntExpression() {
									Children = {
										a, d
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
			 * if (a != b && a != c) {
			 *   ret = false;
			 * } else {
			 *   ret = true;
			 * }
			 */
			if (ParameterCount == 3) {
				trees.Add(new SyntaxTree(new IfStatement() {
					Children = {
						new BoolAndExpression() {
							Children = {
								new BoolNotEqualIntExpression() {
									Children = {
										a, b
									}
								},
								new BoolNotEqualIntExpression() {
									Children = {
										a, c
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
						new BoolAndExpression() {
							Children = {
								new BoolAndExpression() {
									Children = {
										new BoolNotEqualIntExpression() {
											Children = {
												a, b
											}
										},
										new BoolNotEqualIntExpression() {
											Children = {
												a, c
											}
										}
									}
								},
								new BoolNotEqualIntExpression() {
									Children = {
										a, d
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

			/**
			 * Solution 3 (for 3 parameters):
			 * ret = a == b || a == c;
			 */
			if (ParameterCount == 3) {
				trees.Add(new SyntaxTree(new BoolAssignmentStatement() {
					Children = {
						ret,
						new BoolOrExpression() {
							Children = {
								new BoolEqualIntExpression() {
									Children = {
										a, b
									}
								},
								new BoolEqualIntExpression() {
									Children = {
										a, c
									}
								}
							}
						}
					}
				}));
			}
			else if (ParameterCount == 4) {
				trees.Add(new SyntaxTree(new BoolAssignmentStatement() {
					Children = {
						ret,
						new BoolOrExpression() {
							Children = {
								new BoolOrExpression() {
									Children = {
										new BoolEqualIntExpression() {
											Children = {
												a, b
											}
										},
										new BoolEqualIntExpression() {
											Children = {
												a, c
											}
										}
									}
								},
								new BoolEqualIntExpression() {
									Children = {
										a, d
									}
								}
							}
						}
					}
				}));
			}

			return trees;
		}
	}
}
