using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Problems {
	abstract class CodingProblem : Problem {

		public override Type RootType => typeof(void);

		public CodingProblem(int parameterCount, bool initialize = true) 
			: base(parameterCount, initialize) {
		}

		protected override void AddTreeNodes(TreeNodeRepository repository) {
			base.AddTreeNodes(repository);
			if (ReturnType == typeof(int)) {
				repository.Add(new IntIdentifierExpression("ret", true));
				repository.Add(new IntAssignmentStatement());
			} else {
				repository.Add(new BoolIdentifierExpression("ret", true));
				repository.Add(new BoolAssignmentStatement());
			}
			if (ReturnType == typeof(int) || ParameterType == typeof(int)) {
				repository.Add(
					new BoolEqualIntExpression(),
					new BoolNotEqualIntExpression(),
					new BoolGreaterEqualIntExpression(),
					new BoolGreaterThanIntExpression(),
					new BoolLessEqualIntExpression(),
					new BoolLessThanIntExpression(),
					new ForCountStatement());
			} 
			repository.Add(new BoolAndExpression(),
				new BoolFalseExpression(),
				new BoolNotExpression(),
				new BoolOrExpression(),
				new BoolTrueExpression(),
				new BoolXorExpression(),
				new BoolEqualBoolExpression(),
				new BoolNotEqualBoolExpression(),
				new IntAdditionExpression(),
				new IntSubtractionExpression(),
				new IntMultiplicationExpression(),
				new IntDivisionExpression(),
				new IntModuloExpression(),
				new IfStatement());
		}

		protected override CompilationUnitSyntax GetCodeTemplate() {
			return SyntaxFactory.CompilationUnit()
				.WithUsings(SyntaxFactory.List<UsingDirectiveSyntax>(
					new UsingDirectiveSyntax[]{
						SyntaxFactory.UsingDirective(
							SyntaxFactory.IdentifierName("System")),
						SyntaxFactory.UsingDirective(
							SyntaxFactory.QualifiedName(
								SyntaxFactory.IdentifierName("System"),
								SyntaxFactory.IdentifierName("Reflection"))),
						SyntaxFactory.UsingDirective(
							SyntaxFactory.QualifiedName(
								SyntaxFactory.IdentifierName("PatternGuidedGP"),
								SyntaxFactory.IdentifierName("Pangea"))),
						SyntaxFactory.UsingDirective(
							SyntaxFactory.QualifiedName(
								SyntaxFactory.IdentifierName("PatternGuidedGP"),
								SyntaxFactory.IdentifierName("Util")))
					}))
				.WithMembers(
					SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
						SyntaxFactory.ClassDeclaration("ProblemClass")
						.WithMembers(
							SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
								SyntaxFactory.MethodDeclaration(
									SyntaxFactory.PredefinedType(
										SyntaxFactory.Token(SyntaxKind.BoolKeyword))
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
														SyntaxFactory.Token(SyntaxKind.BoolKeyword)))
											}))
										.WithAdditionalAnnotations(new SyntaxAnnotation("ParameterList")))
								.WithBody(
									SyntaxFactory.Block(
										SyntaxFactory.LocalDeclarationStatement(
											SyntaxFactory.VariableDeclaration(
												SyntaxFactory.PredefinedType(
													SyntaxFactory.Token(SyntaxKind.IntKeyword))
												.WithAdditionalAnnotations(new SyntaxAnnotation("ReturnType")))
											.WithVariables(
												SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
													SyntaxFactory.VariableDeclarator(
														SyntaxFactory.Identifier("ret"))
													.WithInitializer(
														SyntaxFactory.EqualsValueClause(
															SyntaxFactory.DefaultExpression(
																SyntaxFactory.PredefinedType(
																	SyntaxFactory.Token(SyntaxKind.IntKeyword))
																.WithAdditionalAnnotations(new SyntaxAnnotation("ReturnType"))
														)
														))))),
										SyntaxFactory.TryStatement(
											SyntaxFactory.SingletonList<CatchClauseSyntax>(
												SyntaxFactory.CatchClause()
												.WithDeclaration(
													SyntaxFactory.CatchDeclaration(
														SyntaxFactory.IdentifierName("Exception"))
													.WithIdentifier(
														SyntaxFactory.Identifier("ex")))))
										.WithBlock(
											SyntaxFactory.Block(
												SyntaxFactory.Block()
													.WithAdditionalAnnotations(new SyntaxAnnotation("SyntaxPlaceholder")))),
										SyntaxFactory.ReturnStatement(
											SyntaxFactory.IdentifierName("ret"))))
										)
									)
								)
							)
				.NormalizeWhitespace();
		}
	}
}
