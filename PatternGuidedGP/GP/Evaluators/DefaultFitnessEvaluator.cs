using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Evaluators {
	class DefaultFitnessEvaluator : ProgramFitnessEvaluator {

		protected override FitnessResult CalculateFitness(Individual individual, TestSuite testSuite, object[] results) {
			int positive = 0;
			for (int i = 0; i < testSuite.TestCases.Count; i++) {
				Logger.WriteLine(4, "Test case " + i + ": " + testSuite.TestCases[i].Result + " = " + results[i]);
				if (testSuite.TestCases[i].Result.Equals(results[i])) {
					positive++;
				}
			}
			return new FitnessResult(1 - ((double)positive / (double)testSuite.TestCases.Count));
		}
	}
}
