using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class FloatMultiplicationExpression : BinaryExpression<float, float, float> {
		public override string Description => "*";
		public override int OperatorId => 304;

		public override SyntaxKind GetKind() {
			return SyntaxKind.MultiplyExpression;
		}

		public override bool IsInvertible => true;

		public override IEnumerable<object> Invert(object desiredValue, int k, object complementValue, out bool ambiguous) {
			var desired = (float)desiredValue;
			var complement = (float)complementValue;
			if (complement != 0) {
				ambiguous = false;
				return new object[] { desired / complement };
			}
			else {
				if (desired == 0) {
					ambiguous = true;
				} else {
					ambiguous = false;
				}
				return Enumerable.Empty<object>();
			}
		}
	}
}
