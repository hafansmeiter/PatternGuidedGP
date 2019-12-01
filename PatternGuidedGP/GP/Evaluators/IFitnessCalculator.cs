using PatternGuidedGP.GP.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Evaluators {
	interface IFitnessCalculator {
		FitnessResult CalculateFitness(Individual individual, TestSuite testSuite, object[] results);
	}
}
