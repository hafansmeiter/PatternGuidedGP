using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	public enum Operators : int {
		// to be done...
		/** bool */
		BoolAnd = 101,
		BoolAssignment = 102,
		BoolEqualBool = 103,
		BoolEqualInt = 105,

		StringAssignment = 401,
		// statements
		ForStatement = 001,
		IfStatement = 002,
	}
}
