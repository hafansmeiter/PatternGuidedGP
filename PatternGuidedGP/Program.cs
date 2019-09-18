using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;

namespace PatternGuidedGP {
	class Program {
		static void Main(string[] args) {
	
			CompilationUnitSyntax template = SyntaxFactory.CompilationUnit()
				.WithUsings(
					SyntaxFactory.SingletonList<UsingDirectiveSyntax>(
						SyntaxFactory.UsingDirective(
							SyntaxFactory.IdentifierName("System"))))
				.WithMembers(
					SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
						SyntaxFactory.ClassDeclaration("C")
						.WithMembers(
							SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
								SyntaxFactory.MethodDeclaration(
									SyntaxFactory.PredefinedType(
										SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
									SyntaxFactory.Identifier("methodName"))
								.WithAdditionalAnnotations(new SyntaxAnnotation("methodName"))
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
														SyntaxFactory.Token(SyntaxKind.BoolKeyword))),
												SyntaxFactory.Token(SyntaxKind.CommaToken),
												SyntaxFactory.Parameter(
													SyntaxFactory.Identifier("b"))
												.WithType(
													SyntaxFactory.PredefinedType(
														SyntaxFactory.Token(SyntaxKind.BoolKeyword)))})))
									.WithAdditionalAnnotations(new SyntaxAnnotation("parameterList"))
								.WithBody(
									SyntaxFactory.Block(
											SyntaxFactory.ReturnStatement(
												SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)
												.WithAdditionalAnnotations(new SyntaxAnnotation("returnValue"))
											)
										.WithAdditionalAnnotations(new SyntaxAnnotation("methodBlock"))
									)
								)
							))))
				.NormalizeWhitespace();

			//Console.WriteLine(template.ToString());

			var generator = new KozaTreeGeneratorFull();
			var repository = new TreeNodeRepository();
			repository.Add(new BoolAndExpression(),
				new BoolFalseExpression(),
				new BoolNotExpression(),
				new BoolOrExpression(),
				new BoolTrueExpression(),
				new BoolXorExpression(),
				new BoolIdentifierExpression("a"),
				new BoolIdentifierExpression("b"));
			generator.setTreeNodeRepository(repository);

			for (int i = 0; i < 20; i++) {
				var tree = generator.GenerateTree(5);
				var syntaxRoot = tree.Root.GenerateSyntax();

				var returnValueNode = template.GetAnnotatedNodes("returnValue").First();
				var newSyntax = template.ReplaceNode(returnValueNode, syntaxRoot);

				Console.WriteLine(newSyntax.ToString());
			}

			Console.ReadKey();
		}
	}
}
