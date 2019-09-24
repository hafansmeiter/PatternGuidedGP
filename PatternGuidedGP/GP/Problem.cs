using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP {
	class Problem {
		public IFitnessEvaluator FitnessEvaluator { get; set; }
		public TestSuite TestSuite { get; set; }

		public void Evaluate(Population population) {
			foreach (var individual in population.Individuals) {
				if (!individual.FitnessEvaluated) {
					double fitness = FitnessEvaluator.Evaluate(individual, TestSuite);
					individual.Fitness = fitness;
				}
			}
		}
	}
}
