using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PatternGuidedGP {
	class Program {
		static void Main(string[] args) {
			var compilationUnit = SyntaxFactory.CompilationUnit()
				.WithMembers(
					SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
						SyntaxFactory.ClassDeclaration("C")
						.WithMembers(
							SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
								SyntaxFactory.MethodDeclaration(
									SyntaxFactory.PredefinedType(
										SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
									SyntaxFactory.Identifier("allEqual"))
								.WithModifiers(
									SyntaxFactory.TokenList(
										new[]{
											SyntaxFactory.Token(SyntaxKind.PublicKeyword),
											SyntaxFactory.Token(SyntaxKind.StaticKeyword)}))
								.WithParameterList(
									SyntaxFactory.ParameterList(
										SyntaxFactory.SeparatedList<ParameterSyntax>(
											new SyntaxNodeOrToken[]{
												SyntaxFactory.Parameter(
													SyntaxFactory.Identifier("a"))
												.WithType(
													SyntaxFactory.PredefinedType(
														SyntaxFactory.Token(SyntaxKind.IntKeyword))),
												SyntaxFactory.Token(SyntaxKind.CommaToken),
												SyntaxFactory.Parameter(
													SyntaxFactory.Identifier("b"))
												.WithType(
													SyntaxFactory.PredefinedType(
														SyntaxFactory.Token(SyntaxKind.IntKeyword))),
												SyntaxFactory.Token(SyntaxKind.CommaToken),
												SyntaxFactory.Parameter(
													SyntaxFactory.Identifier("c"))
												.WithType(
													SyntaxFactory.PredefinedType(
														SyntaxFactory.Token(SyntaxKind.IntKeyword)))})))
								.WithBody(
									SyntaxFactory.Block(
										SyntaxFactory.SingletonList<StatementSyntax>(
											SyntaxFactory.ReturnStatement(
												SyntaxFactory.BinaryExpression(
													SyntaxKind.LogicalAndExpression,
													SyntaxFactory.BinaryExpression(
														SyntaxKind.EqualsExpression,
														SyntaxFactory.IdentifierName("a"),
														SyntaxFactory.IdentifierName("b")),
													SyntaxFactory.BinaryExpression(
														SyntaxKind.EqualsExpression,
														SyntaxFactory.IdentifierName("b"),
														SyntaxFactory.IdentifierName("c")))))))))))
				.NormalizeWhitespace();

			if (typeof(void) == typeof(int))
				Console.WriteLine(compilationUnit.ToString());
			Console.ReadKey();
		}
	}
}
