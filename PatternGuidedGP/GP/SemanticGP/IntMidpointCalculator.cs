using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	class IntMidpointCalculator : IMidpointCalculator {

		// Algorithm details: Pawlak et al. - Competent Geometric Semantic GP page 187 
		public Semantics GetMidpoint(Semantics semantics1, Semantics semantics2) {
			int length = semantics1.Length;
			Semantics midpoint = new Semantics(length);
			for (int i = 0; i < length; i++) {
				int s1 = (int)semantics1[i];
				int s2 = (int)semantics2[i];
				midpoint[i] = (s1 + s2) / 2;
			}
			return midpoint;
		}
	}
}
