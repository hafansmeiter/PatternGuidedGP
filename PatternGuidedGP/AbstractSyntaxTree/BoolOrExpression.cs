using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class BoolOrExpression : BinaryExpression<bool, bool, bool> {
		public override string Description => "||";

		public override SyntaxKind GetKind() {
			return SyntaxKind.LogicalOrExpression;
		}

		public override bool IsInvertible => true;

		public override IEnumerable<object> Invert(object desiredValue, int k, object complementValue, out bool ambiguous) {
			bool desired = (bool)desiredValue;
			bool complement = (bool)complementValue;
			if (!complement) {
				ambiguous = false;
				return new object[] { desired };
			}
			else {
				if (complement && desired) {
					ambiguous = true;
				}
				else {
					ambiguous = false;
				}
				return Enumerable.Empty<object>();
			}
		}
	}
}
