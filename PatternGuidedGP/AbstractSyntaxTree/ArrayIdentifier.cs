using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class ArrayIdentifier<T> : IdentifierExpression<T> {
		public override Type[] ChildTypes => new Type[] { typeof(int) };
		public override int RequiredTreeDepth => 1; // index is not a subtree as it is not considered as logical tree node
		public Expression<int> Index => Children[0] as Expression<int>;

		public ArrayIdentifier(string name, bool assignable = false) : base(name, assignable) {
			Name = name;
			IsAssignable = assignable;
		}

		protected override CSharpSyntaxNode GenerateSyntax() {
			return SyntaxFactory.ElementAccessExpression(
					SyntaxFactory.IdentifierName(Name))
					.WithArgumentList(
						SyntaxFactory.BracketedArgumentList(
							SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
								SyntaxFactory.Argument(
									(ExpressionSyntax) Index.GetSyntaxNode()))));
		}
	}
}
