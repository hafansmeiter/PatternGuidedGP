﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	interface IGeometricCalculator {
		Semantics GetMidpoint(Semantics semantics1, Semantics semantics2);
	}
}
