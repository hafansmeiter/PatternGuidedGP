using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class BoolFalseExpression : NullaryExpression<bool> {
		public override bool IsVariable => false;

		public override CSharpSyntaxNode GenerateSyntax() {
			return SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
		}
	}
}
