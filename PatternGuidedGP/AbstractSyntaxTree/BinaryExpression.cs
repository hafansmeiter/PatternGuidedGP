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
		public override bool IsTerminal => false;
		public override bool IsVariable => false;
		public override bool IsChildCountFixed => true;

		public Expression<TLeft> Left {
			get {
				return Children[0] as Expression<TLeft>;
			}
		}

		public Expression<TRight> Right {
			get {
				return Children[1] as Expression<TRight>;
			}
		}

		public override CSharpSyntaxNode GenerateSyntax() {
			return SyntaxFactory.BinaryExpression(GetKind(), 
				(ExpressionSyntax) Left.GenerateSyntax(), (ExpressionSyntax) Right.GenerateSyntax());
		}

		public abstract SyntaxKind GetKind();
	}
}
