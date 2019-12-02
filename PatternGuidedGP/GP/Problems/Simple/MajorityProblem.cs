﻿using PatternGuidedGP.AbstractSyntaxTree;
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
		public override Type ParameterType => typeof(bool);

		public MajorityProblem(int n) : base(n) {
		}

		protected override TestSuite GetTestSuite() {
			return new BoolTestSuiteGenerator().Create(ParameterCount, parameters => {
				int trues = 0;
				foreach (var par in parameters) {
					if ((bool)par) {
						trues++;
					}
				}
				return trues > ParameterCount / 2;
			});
		}
	}
}