using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Evaluators {
	class FitnessResult {
		public double Fitness { get; set; }

		public FitnessResult(double fitness) {
			Fitness = fitness;
		}
	}
}
