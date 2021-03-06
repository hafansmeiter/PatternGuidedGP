﻿using PatternGuidedGP.GP.Problems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP {
	interface IAlgorithm {
		bool IsSolutionFound();
		Individual Run(Problem problem);
	}
}
