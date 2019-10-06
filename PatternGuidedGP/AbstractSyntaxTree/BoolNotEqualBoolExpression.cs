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
	}
}
