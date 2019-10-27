using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	interface ISemanticsProvider {
		bool IsSemanticsEvaluated { get; set; }
		Semantics Semantics { get; set; }
	}
}
