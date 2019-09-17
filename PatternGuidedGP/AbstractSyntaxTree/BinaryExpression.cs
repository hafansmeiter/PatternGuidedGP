using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.AbstractSyntaxTree.SyntaxGenerator.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class BinaryExpression<TResult, TLeft, TRight> : Expression<TResult>, ISyntaxKindProvider {
		public override Type[] ChildTypes => new[] { typeof(TLeft), typeof(TRight) };

		public override IEnumerable<TreeItem> Children {
			get {
				yield return Left;
				yield return Right;
			}
		}

		public Expression<TLeft> Left { get; set; }
		public Expression<TRight> Right { get; set; }

		public override CSharpSyntaxNode GenerateSyntax() {
			return SyntaxFactory.BinaryExpression(GetKind(), 
				(ExpressionSyntax) Left.GenerateSyntax(), (ExpressionSyntax) Right.GenerateSyntax());
		}

		public abstract SyntaxKind GetKind();
	}
}
