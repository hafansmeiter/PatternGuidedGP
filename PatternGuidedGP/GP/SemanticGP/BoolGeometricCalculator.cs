﻿using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	class BoolGeometricCalculator : IGeometricCalculator {

		// Algorithm details: Pawlak et al. - Competent Geometric Semantic GP page 187 
		public Semantics GetMidpoint(Semantics semantics1, Semantics semantics2) {
			int length = semantics1.Length;
			var midpoint = new Semantics(length);
			var random = GetRandom(length);
			for (int i = 0; i < length; i++) {
				bool s1 = semantics1[i] != null ? (bool)semantics1[i] : false;	// use false if no semantics evaluated
				bool s2 = semantics2[i] != null ? (bool)semantics2[i] : false;
				bool sx = (bool)random[i];
				midpoint[i] = (s1 && sx) || (!sx && s2);
			}
			return midpoint;
		}

		private Semantics GetRandom(int length) {
			var semantics = new Semantics(length);
			for (int i = 0; i < length; i++) {
				semantics[i] = RandomValueGenerator.Instance.GetBool();
			}
			return semantics;
		}
	}
}
