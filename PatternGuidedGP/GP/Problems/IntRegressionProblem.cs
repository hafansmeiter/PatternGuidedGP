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
using PatternGuidedGP.GP.Tests;

namespace PatternGuidedGP.GP.Problems {
	abstract class IntRegressionProblem : Problem {

		public override Type RootType => typeof(int);

		public IntRegressionProblem(int parameterCount) : base(parameterCount) {
		}

		protected override void AddTreeNodes(TreeNodeRepository repository) {
			repository.Add(new IntAdditionExpression(),
				new IntSubtractionExpression(),
				new IntMultiplicationExpression(),
				new IntDivisionExpression(),
				new IntModuloExpression());

			for (int i = 0; i < _parameterCount; i++) {
				repository.Add(new IntIdentifierExpression(((char)('a' + i)).ToString()));
			}
		}

		protected override CompilationUnitSyntax GetCodeTemplate() {
			return SyntaxFactory.CompilationUnit()
				.WithUsings(
					SyntaxFactory.SingletonList<UsingDirectiveSyntax>(
						SyntaxFactory.UsingDirective(
							SyntaxFactory.IdentifierName("System"))))
				.WithMembers(
					SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
						SyntaxFactory.ClassDeclaration("ProblemClass")
						.WithMembers(
							SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
								SyntaxFactory.MethodDeclaration(
									SyntaxFactory.PredefinedType(
										SyntaxFactory.Token(SyntaxKind.IntKeyword))
									.WithAdditionalAnnotations(new SyntaxAnnotation("ReturnType")),
									SyntaxFactory.Identifier("Test"))
								.WithAdditionalAnnotations(new SyntaxAnnotation("MethodDeclaration"))
								.WithModifiers(
									SyntaxFactory.TokenList(
										new[]{
											SyntaxFactory.Token(SyntaxKind.PublicKeyword),
											SyntaxFactory.Token(SyntaxKind.StaticKeyword)}))
								.WithParameterList(
									SyntaxFactory.ParameterList(
										SyntaxFactory.SeparatedList<ParameterSyntax>(
											new SyntaxNodeOrToken[] {
												SyntaxFactory.Parameter(
													SyntaxFactory.Identifier("a"))
												.WithType(
													SyntaxFactory.PredefinedType(
														SyntaxFactory.Token(SyntaxKind.IntKeyword)))
											}))
										.WithAdditionalAnnotations(new SyntaxAnnotation("ParameterList")))
								.WithBody(
									SyntaxFactory.Block(
											SyntaxFactory.ReturnStatement(
												SyntaxFactory.IdentifierName("a")
												.WithAdditionalAnnotations(new SyntaxAnnotation("SyntaxPlaceholder"))
										)
									)
									.WithAdditionalAnnotations(new SyntaxAnnotation("MethodBlock"))
								)
							))))
				.NormalizeWhitespace();
		}
	}
}
