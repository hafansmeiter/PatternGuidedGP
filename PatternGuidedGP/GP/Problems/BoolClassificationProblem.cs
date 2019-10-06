﻿using System;
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
	abstract class BoolClassificationProblem : Problem {
		public override Type RootType => typeof(bool);
		public override Type ParameterType => typeof(bool);
		public override Type ReturnType => typeof(bool);

		public BoolClassificationProblem(int parameterCount) : base(parameterCount) {
		}

		protected override void AddTreeNodes(TreeNodeRepository repository) {
			repository.Add(new BoolAndExpression(),
				new BoolFalseExpression(),
				new BoolNotExpression(),
				new BoolOrExpression(),
				new BoolTrueExpression(),
				new BoolXorExpression(),
				new BoolEqualBoolExpression(),
				new BoolNotEqualBoolExpression());
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
											SyntaxFactory.ReturnStatement(
												SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)
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
