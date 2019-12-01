using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class BoolLessEqualFloatExpression : BinaryExpression<bool, float, float> {
		public override string Description => "<=";
		public override int OperatorId => 110;

		public override SyntaxKind GetKind() {
			return SyntaxKind.LessThanOrEqualExpression;
		}

		public override bool IsInvertible => false;

		public override IEnumerable<object> Invert(object desiredValue, int k, object complementValue, out bool ambiguous) {
			ambiguous = true;
			return Enumerable.Empty<object>();
		}
	}
}
