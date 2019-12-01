using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Util;

namespace PatternGuidedGP.GP.Evaluators {
	class AbsoluteDistanceFitnessCalculator : IFitnessCalculator {
		public FitnessResult CalculateFitness(Individual individual, TestSuite testSuite, object[] results) {
			double distance = 0;
			for (int i = 0; i < testSuite.TestCases.Count; i++) {
				distance += Math.Abs(testSuite.TestCases[i].Result.ToNumeric() - results[i].ToNumeric());
				Logger.WriteLine(4, "Test case " + i + ": " + 
					testSuite.TestCases[i].Result + " = " + results[i] +
					" (" + distance + ")");
			}
			return new FitnessResult(distance);
		}
	}
}
