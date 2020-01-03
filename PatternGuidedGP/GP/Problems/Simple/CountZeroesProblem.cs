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
	// Use parameters 1,2,3 instead of 0,1,2 in order to reduce 
	// occurrence of DivideByZeroExceptions => count ones
	class CountZeroesProblem : SimpleCodingProblem {
		public override Type ReturnType => typeof(int);
		public override Type ParameterType => typeof(int);

		public CountZeroesProblem(int parameterCount) : base(parameterCount) {
		}

		protected override TestSuite GetTestSuite() {
			return new IntTestSuiteGenerator().Create(ParameterCount, parameters => {
				int zeroes = 0;
				for (int i = 0; i < parameters.Length; i++) {
					if ((int) parameters[i] == 1) {
						zeroes++;
					}
				}
				return zeroes;
			});
		}

		protected override IEnumerable<SyntaxTree> CreateOptimalSolutions() {
			IList<SyntaxTree> trees = new List<SyntaxTree>();

			var a = new IntIdentifierExpression("a");
			var b = new IntIdentifierExpression("b");
			var c = new IntIdentifierExpression("c");
			var d = new IntIdentifierExpression("d");
			var ret = new IntIdentifierExpression("ret");
			var one = new IntLiteralExpression(1);
			var two = new IntLiteralExpression(2);
			var three = new IntLiteralExpression(3);
			var four = new IntLiteralExpression(4);

			/**
			 * Solution 1 (for 3 parameters):
			 * if (a == 1) {
			 *   ret = ret + 1;
			 * } else {
			 *   ret = ret;
			 * }
			 * if (b == 1) {
			 *   ret = ret + 1;
			 * } else {
			 *   ret = ret;
			 * }
			 * if (c == 1) {
			 *   ret = ret + 1;
			 * } else {
			 *   ret = ret;
			 * }
			 */
			if (ParameterCount == 3) {
				trees.Add(new SyntaxTree(new IfStatement() {
					Children = {
						new BoolEqualIntExpression() {
							Children = {
								a, one
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
						},
						new IntAssignmentStatement() {
							Children = {
								ret,
								ret
							}
						}
					}
				},
				new IfStatement() {
					Children = {
						new BoolEqualIntExpression() {
							Children = {
								b, one
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
						},
						new IntAssignmentStatement() {
							Children = {
								ret,
								ret
							}
						}
					}
				},
				new IfStatement() {
					Children = {
						new BoolEqualIntExpression() {
							Children = {
								c, one
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
						},
						new IntAssignmentStatement() {
							Children = {
								ret,
								ret
							}
						}
					}
				}));
			}
			else if (ParameterCount == 4) {
				trees.Add(new SyntaxTree(new IfStatement() {
					Children = {
						new BoolEqualIntExpression() {
							Children = {
								a, one
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
						},
						new IntAssignmentStatement() {
							Children = {
								ret,
								ret
							}
						}
					}
				},
				new IfStatement() {
					Children = {
						new BoolEqualIntExpression() {
							Children = {
								b, one
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
						},
						new IntAssignmentStatement() {
							Children = {
								ret,
								ret
							}
						}
					}
				},
				new IfStatement() {
					Children = {
						new BoolEqualIntExpression() {
							Children = {
								c, one
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
						},
						new IntAssignmentStatement() {
							Children = {
								ret,
								ret
							}
						}
					}
				},
				new IfStatement() {
					Children = {
						new BoolEqualIntExpression() {
							Children = {
								d, one
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
						},
						new IntAssignmentStatement() {
							Children = {
								ret,
								ret
							}
						}
					}
				}));
			}
			return trees;
		}
	}
}
