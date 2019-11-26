using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class FloatIdentifierExpression : IdentifierExpression<float> {
		public FloatIdentifierExpression(string name, bool targetVariable = false) : base(name, targetVariable) {
		}
	}
}
