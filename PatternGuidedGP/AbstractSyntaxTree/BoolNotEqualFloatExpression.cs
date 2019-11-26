using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class BoolNotEqualFloatExpression : BinaryExpression<bool, float, float> {
		public override string Description => "!=";

		public override SyntaxKind GetKind() {
			return SyntaxKind.NotEqualsExpression;
		}

		public override bool IsInvertible => true;

		public override IEnumerable<object> Invert(object desiredValue, int k, object complementValue, out bool ambiguous) {
			var desired = (bool)desiredValue;
			var complement = (float)complementValue;
			if (!desired) {
				ambiguous = false;
				return new object[] { complement };
			}
			else {
				ambiguous = true;
				return Enumerable.Empty<object>();
			}
		}
	}
}
