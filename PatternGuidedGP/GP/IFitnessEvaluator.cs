using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP {
	interface IFitnessEvaluator {
		double Evaluate(Individual individual);
	}
}
