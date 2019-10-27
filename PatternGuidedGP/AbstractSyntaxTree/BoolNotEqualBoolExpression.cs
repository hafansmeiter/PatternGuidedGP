using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class BoolNotEqualBoolExpression : BinaryExpression<bool, bool, bool> {
		public override string Description => "!=";

		public override SyntaxKind GetKind() {
			return SyntaxKind.NotEqualsExpression;
		}

		public override bool IsInvertible => true;

		public override IEnumerable<object> Invert(object desiredValue, int k, object complementValue, out bool ambiguous) {
			bool desired = (bool)desiredValue;
			bool complement = (bool)complementValue;
			ambiguous = false;
			if (desired) {
				return new object[] { !complement };
			}
			else {
				return new object[] { complement };
			}
		}
	}
}
