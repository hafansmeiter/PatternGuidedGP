﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP.Tests;

namespace PatternGuidedGP.GP.Problems {
	class ContainsFirstProblem : CodingProblem {
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
	}
}
