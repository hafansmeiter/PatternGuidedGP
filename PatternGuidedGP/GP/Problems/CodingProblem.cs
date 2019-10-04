﻿using Microsoft.CodeAnalysis;
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

		public CodingProblem(int parameterCount) : base(parameterCount) {
		}

		protected override void AddTreeNodes(TreeNodeRepository repository) {
			repository.Add(new BoolAndExpression(),
				new BoolFalseExpression(),
				new BoolNotExpression(),
				new BoolOrExpression(),
				new BoolTrueExpression(),
				new BoolXorExpression(),
				new BoolEqualBoolExpression(),
				new BoolNotEqualBoolExpression(),
				new BoolEqualIntExpression(),
				new BoolNotEqualIntExpression(),
				new BoolGreaterEqualIntExpression(),
				new BoolGreaterThanIntExpression(),
				new BoolLessEqualIntExpression(),
				new BoolLessThanIntExpression(),
				new IntAdditionExpression(),
				new IntSubtractionExpression(),
				new IntMultiplicationExpression(),
				new IntDivisionExpression(),
				new IntModuloExpression(),
				new BlockStatement(),
				new IfStatement(),
				new ForCountStatement());
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
										SyntaxFactory.Block()
											.WithAdditionalAnnotations(new SyntaxAnnotation("SyntaxPlaceholder")),
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