using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class BoolXorExpression : BinaryExpression<bool, bool, bool> {
		public override string Description => "XOR";
		public override int OperatorId => 119;

		public override SyntaxKind GetKind() {
			return SyntaxKind.ExclusiveOrExpression;
		}

		public override bool IsInvertible => true;

		public override IEnumerable<object> Invert(object desiredValue, int k, object complementValue, out bool ambiguous) {
			bool desired = (bool)desiredValue;
			bool complement = (bool)complementValue;
			ambiguous = false;
			if (desired) {
				return new object[] { !complement };
			} else {
				return new object[] { complement };
			}
		}
	}
}
