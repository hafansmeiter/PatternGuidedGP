using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	class IntGeometricCalculator : IGeometricCalculator {

		// Algorithm details: Pawlak et al. - Competent Geometric Semantic GP page 187 
		public Semantics GetMidpoint(Semantics semantics1, Semantics semantics2) {
			int length = semantics1.Length;
			Semantics midpoint = new Semantics(length);
			for (int i = 0; i < length; i++) {
				int s1 = semantics1[i] != null ? (int)semantics1[i] : 0;	// use 0 if no semantics evaluated
				int s2 = semantics2[i] != null ? (int)semantics2[i] : 0;
				midpoint[i] = (s1 + s2) / 2;
			}
			return midpoint;
		}
	}
}
