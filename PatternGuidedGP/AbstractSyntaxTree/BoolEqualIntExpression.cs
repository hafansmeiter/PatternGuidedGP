using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class BoolEqualIntExpression : BinaryExpression<bool, int, int> {
		public override string Description => "==";
		public override int OperatorId => 105;

		public override SyntaxKind GetKind() {
			return SyntaxKind.EqualsExpression;
		}

		public override bool IsInvertible => true;

		public override IEnumerable<object> Invert(object desiredValue, int k, object complementValue, out bool ambiguous) {
			var desired = (bool)desiredValue;
			var complement = (int)complementValue;
			if (desired) {
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
