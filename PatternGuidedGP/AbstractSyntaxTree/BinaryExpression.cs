using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.AbstractSyntaxTree.SyntaxGenerator.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class BinaryExpression<TResult, TLeft, TRight> : Expression<TResult> {
		public override Type[] ChildTypes => new[] { typeof(TLeft), typeof(TRight) };
		public override bool IsTerminal => false;
		public override bool IsVariable => false;
		public override int RequiredTreeDepth => 2;

		public Expression<TLeft> Left => Children[0] as Expression<TLeft>;
		public Expression<TRight> Right => Children[1] as Expression<TRight>;

		protected override CSharpSyntaxNode GenerateSyntax() {
			return SyntaxFactory.ParenthesizedExpression(
				SyntaxFactory.BinaryExpression(GetKind(),
					(ExpressionSyntax)Left.GetSyntaxNode(),
					(ExpressionSyntax)Right.GetSyntaxNode()));
		}

		public abstract SyntaxKind GetKind();

		public override object GetComplementValue(int k, int semanticsIndex) {
			if (k == 0) {
				if (Right.SemanticsEvaluated) {
					return Right.Semantics[semanticsIndex];
				}
			} else if (k == 1) {
				if (Left.SemanticsEvaluated) {
					return Left.Semantics[semanticsIndex];
				}
			}
			return null;
		}
	}
}
