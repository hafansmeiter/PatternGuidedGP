using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class BoolTrueExpression : NullaryExpression<bool> {
		public override CSharpSyntaxNode GenerateSyntax() {
			return SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression);
		}
	}
}
