using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Operators {
	class MultiRandomMutator : MultiRandomOperator<IMutator>, IMutator {
		public bool Mutate(Individual individual) {
			return GetRandom().Mutate(individual);
		}
	}
}
