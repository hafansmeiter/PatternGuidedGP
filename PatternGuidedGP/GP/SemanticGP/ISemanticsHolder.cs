using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	interface ISemanticsHolder {
		bool SemanticsEvaluated { get; set; }
		Semantics Semantics { get; set; }
	}
}
