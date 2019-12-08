using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	public enum Operators : int {
		/** bool */
		BoolAnd = 101,
		BoolAssignment = 102,
		BoolEqualBool = 103,

		StringAssignment = 401
	}
}
