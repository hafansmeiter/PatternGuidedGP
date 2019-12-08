using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class PrintStatement : Statement {
		public override int RequiredTreeDepth => 2;
		public override string Description => "print";
		public override Type[] ChildTypes => new[] { typeof(string) };
		public override int OperatorId => 003;

		public Expression<string> Text => Children[0] as Expression<string>;

		protected override CSharpSyntaxNode GenerateSyntax() {
			return SyntaxFactory.InvocationExpression(
					SyntaxFactory.MemberAccessExpression(
						SyntaxKind.SimpleMemberAccessExpression,
						SyntaxFactory.MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							SyntaxFactory.GenericName(
								SyntaxFactory.Identifier("Singleton"))
							.WithTypeArgumentList(
								SyntaxFactory.TypeArgumentList(
									SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
										SyntaxFactory.IdentifierName("StandardOut")))),
							SyntaxFactory.IdentifierName("Instance")),
						SyntaxFactory.IdentifierName("Print")))
					.WithArgumentList(
						SyntaxFactory.ArgumentList(
							SyntaxFactory.SeparatedList<ArgumentSyntax>(
								new ArgumentSyntax[] {
									SyntaxFactory.Argument((ExpressionSyntax) Text.GetSyntaxNode())
								}
							)
						)
					);
		}
	}
}
