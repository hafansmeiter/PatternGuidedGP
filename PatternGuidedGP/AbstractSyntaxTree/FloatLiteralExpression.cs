using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class FloatLiteralExpression : LiteralExpression<float> {

		public FloatLiteralExpression(float value) : base(value) {
		}

		protected override CSharpSyntaxNode GenerateSyntax() {
			return SyntaxFactory.LiteralExpression(
				SyntaxKind.NumericLiteralExpression,
				SyntaxFactory.Literal((float)Value));
		}
	}
}
