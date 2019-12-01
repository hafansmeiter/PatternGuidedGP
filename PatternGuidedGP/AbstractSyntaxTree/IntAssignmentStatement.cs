using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class IntAssignmentStatement : AssignmentStatement<int> {
		public override int OperatorId => 202;
	}
}
