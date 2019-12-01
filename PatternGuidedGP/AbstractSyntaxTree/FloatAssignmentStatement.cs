using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class FloatAssignmentStatement : AssignmentStatement<float> {
		public override int OperatorId => 302;
	}
}
