using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.AbstractSyntaxTree.SyntaxGenerator.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class UnaryExpression<TResult, TChild> : Expression<TResult>, ISyntaxKindProvider {
		public override Type[] ChildTypes => new[] { typeof(TChild) };
		public override bool IsTerminal => false;
		public override bool IsVariable => false;
		public override bool IsChildCountFixed => true;

		public Expression<TChild> Child {
			get {
				return Children[0] as Expression<TChild>;
			}
		}

		protected override CSharpSyntaxNode GenerateSyntax() {
			return SyntaxFactory.PrefixUnaryExpression(
				GetKind(), (ExpressionSyntax)Child.GetSyntaxNode());
		}

		public abstract SyntaxKind GetKind();
	}
}
