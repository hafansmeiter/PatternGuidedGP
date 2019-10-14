using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class IntIdentifierExpression : IdentifierExpression<int> {
		public IntIdentifierExpression(string name, bool targetVariable = false) : base(name, targetVariable) {
		}
	}
}
