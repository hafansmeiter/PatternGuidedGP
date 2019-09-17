using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class BoolGreaterThanIntExpression : BinaryExpression<bool, int, int> {
		public override SyntaxKind GetKind() {
			return SyntaxKind.GreaterThanExpression;
		}
	}
}
