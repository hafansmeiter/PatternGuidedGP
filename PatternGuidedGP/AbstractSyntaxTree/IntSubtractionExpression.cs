using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class IntSubtractionExpression : BinaryExpression<int, int, int> {
		public override string Description => "-";

		public override SyntaxKind GetKind() {
			return SyntaxKind.SubtractExpression;
		}

		public override bool IsInvertible => true;

		public override IEnumerable<object> Invert(object desiredValue, int k, object complementValue, out bool ambiguous) {
			var desired = (int)desiredValue;
			var complement = (int)complementValue;
			ambiguous = false;
			if (k == 0) {
				return new object[] { desired + complement };
			}
			else {
				return new object[] { complement - desired };
			}
		}
	}
}
