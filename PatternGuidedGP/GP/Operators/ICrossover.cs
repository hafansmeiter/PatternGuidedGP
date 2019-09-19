using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Operators {
	interface ICrossover {
		Individual cross(Individual individual1, Individual individual2);
	}
}
