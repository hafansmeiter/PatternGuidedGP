﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Operators {
	interface ICrossover {
		IEnumerable<Individual> Cross(Individual individual1, Individual individual2);
	}
}
