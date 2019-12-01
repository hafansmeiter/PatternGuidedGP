using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Util;

namespace PatternGuidedGP.GP.Evaluators {
	class EqualityFitnessCalculator : IFitnessCalculator {
		public FitnessResult CalculateFitness(Individual individual, TestSuite testSuite, object[] results) {
			int positive = 0;
			for (int i = 0; i < testSuite.TestCases.Count; i++) {
				bool equals = testSuite.TestCases[i].Result.Equals(results[i]);
				if (equals) {
					positive++;
				}
				Logger.WriteLine(4, "Test case " + i + ": " + 
					testSuite.TestCases[i].Result + " = " + results[i] + 
					" (" + equals + ")");
			}
			return new FitnessResult(1 - ((double)positive / (double)testSuite.TestCases.Count));
		}
	}
}
