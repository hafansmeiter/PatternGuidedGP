using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.GP.SemanticGP;
using PatternGuidedGP.GP.Tests;

namespace PatternGuidedGP.GP.Problems {
	abstract class CodingProblem : Problem {
		public override Type RootType => typeof(void);

		public CodingProblem(bool initialize = true) : base(initialize) {
			GeometricCalculator = GetGeometricCalculator();
		}

		public CodingProblem(int n, bool initialize = true) : base(n, initialize) {
			GeometricCalculator = GetGeometricCalculator();
		}

		protected virtual IGeometricCalculator GetGeometricCalculator() {
			if (ReturnType == typeof(bool)) {
				return new BoolGeometricCalculator();
			}
			else if (ReturnType == typeof(int)) {
				return new IntGeometricCalculator();
			}
			return null;
		}
	}
}
