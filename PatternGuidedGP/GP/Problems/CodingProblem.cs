using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.GP.Tests;

namespace PatternGuidedGP.GP.Problems {
	abstract class CodingProblem : Problem {
		public override Type RootType => typeof(void);

		public CodingProblem(int n, bool initialize = true) : base(n, initialize) {
		}
	}
}
