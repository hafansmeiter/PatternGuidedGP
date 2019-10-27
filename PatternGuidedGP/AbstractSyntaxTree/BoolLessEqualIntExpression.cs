using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class BoolLessEqualIntExpression : BinaryExpression<bool, int, int> {
		public override string Description => "<=";

		public override SyntaxKind GetKind() {
			return SyntaxKind.LessThanOrEqualExpression;
		}

		public override bool IsInvertible => true;

		public override IEnumerable<object> Invert(object desiredValue, int k, object complementValue, out bool ambiguous) {
			ambiguous = true;
			return Enumerable.Empty<object>();
		}
	}
}
