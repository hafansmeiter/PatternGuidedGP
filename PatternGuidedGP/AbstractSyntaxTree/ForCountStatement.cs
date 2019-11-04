using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class ForCountStatement : Statement {
		public override string Description => "for";
		public override bool IsTerminal => false;
		public override bool IsVariable => false;
		public override int RequiredTreeDepth => 3;
		public override Type[] ChildTypes => new[] { typeof(int), typeof(void) };

		public Expression<int> Count => Children[0] as Expression<int>;
		public Statement ContentBlock => Children[1] as Statement;

		public int MaxLoops { get; set; } = 10000;

		protected override CSharpSyntaxNode GenerateSyntax() {
			string indexName = "i" + Id;
			return SyntaxFactory.ForStatement(
						SyntaxFactory.Block((StatementSyntax) ContentBlock.GetSyntaxNode()))
					.WithDeclaration(
						SyntaxFactory.VariableDeclaration(
							SyntaxFactory.PredefinedType(
								SyntaxFactory.Token(SyntaxKind.IntKeyword)))
						.WithVariables(
							SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
								SyntaxFactory.VariableDeclarator(
									SyntaxFactory.Identifier(indexName))
								.WithInitializer(
									SyntaxFactory.EqualsValueClause(
										SyntaxFactory.LiteralExpression(
											SyntaxKind.NumericLiteralExpression,
											SyntaxFactory.Literal(0)))))))
					.WithCondition(
						SyntaxFactory.BinaryExpression(
							SyntaxKind.LessThanExpression,
							SyntaxFactory.IdentifierName(indexName),
							SyntaxFactory.InvocationExpression(
								SyntaxFactory.MemberAccessExpression(
									SyntaxKind.SimpleMemberAccessExpression,
									SyntaxFactory.IdentifierName("Math"),
									SyntaxFactory.IdentifierName("Min")))
							.WithArgumentList(
								SyntaxFactory.ArgumentList(
									SyntaxFactory.SeparatedList<ArgumentSyntax>(
										new SyntaxNodeOrToken[]{
											SyntaxFactory.Argument(
												SyntaxFactory.LiteralExpression(
													SyntaxKind.NumericLiteralExpression,
													SyntaxFactory.Literal(MaxLoops))),
											SyntaxFactory.Token(SyntaxKind.CommaToken),
											SyntaxFactory.Argument(
												(ExpressionSyntax) Count.GetSyntaxNode())})))))
					.WithIncrementors(
						SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
							SyntaxFactory.PostfixUnaryExpression(
								SyntaxKind.PostIncrementExpression,
								SyntaxFactory.IdentifierName(indexName))));
		}
	}
}
