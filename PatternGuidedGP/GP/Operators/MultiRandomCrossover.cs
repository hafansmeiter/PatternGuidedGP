using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Operators {
	class MultiRandomCrossover : MultiRandomOperator<ICrossover>, ICrossover {
		public IEnumerable<Individual> Cross(Individual individual1, Individual individual2) {
			return GetRandom().Cross(individual1, individual2);
		}
	}
}
