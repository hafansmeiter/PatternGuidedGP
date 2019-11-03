using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	interface IResultSemanticsOperator {
		bool Operate(Semantics resultSemantics, Individual individual, ISemanticSubTreePool subTreePool, int maxTreeDepth);
	}
}
