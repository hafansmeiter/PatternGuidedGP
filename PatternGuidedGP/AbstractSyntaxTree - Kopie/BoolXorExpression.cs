using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class BoolXorExpression : BinaryExpression<bool, bool, bool> {
		public override CSharpSyntaxNode GenerateSyntax() {
			ExpressionSyntax leftSyntax = (ExpressionSyntax)Left.GenerateSyntax();
			ExpressionSyntax rightSyntax = (ExpressionSyntax)Right.GenerateSyntax();
			return SyntaxFactory.BinaryExpression(
				SyntaxKind.LogicalAndExpression,
				SyntaxFactory.ParenthesizedExpression(
					SyntaxFactory.BinaryExpression(
						SyntaxKind.LogicalOrExpression,
						leftSyntax,
						rightSyntax)),
				SyntaxFactory.PrefixUnaryExpression(
					SyntaxKind.LogicalNotExpression,
					SyntaxFactory.ParenthesizedExpression(
						SyntaxFactory.BinaryExpression(
							SyntaxKind.LogicalAndExpression,
							leftSyntax,
							rightSyntax))));
		}

		public override SyntaxKind GetKind() {
			return SyntaxKind.None;	// not required
		}
	}
}
