using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class StringAssignmentStatement : AssignmentStatement<string> {
		public override int OperatorId => (int) Operators.StringAssignment;
	}
}
