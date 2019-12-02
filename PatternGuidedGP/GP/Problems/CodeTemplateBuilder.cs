using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Problems {
	class CodeTemplateBuilder {

		private CompilationUnitSyntax _template;
		private IList<ParameterSyntax> _parameters;

		public CodeTemplateBuilder() {
			_template = GetCodeTemplate();
			_parameters = new List<ParameterSyntax>();
		}

		public CodeTemplateBuilder AddParameter(Type type, string name, bool isArray = false) {
			if (isArray) {
				_parameters.Add(SyntaxFactory.Parameter(
					SyntaxFactory.Identifier(name))
				.WithType(
					SyntaxFactory.ArrayType(
						SyntaxFactory.PredefinedType(
							SyntaxFactory.Token(GetTypeSyntax(type))))
					.WithRankSpecifiers(
						SyntaxFactory.SingletonList<ArrayRankSpecifierSyntax>(
							SyntaxFactory.ArrayRankSpecifier(
								SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
									SyntaxFactory.OmittedArraySizeExpression()))))));
			} else {
				_parameters.Add(SyntaxFactory.Parameter(SyntaxFactory.Identifier(name))
					.WithType(SyntaxFactory.PredefinedType(
						SyntaxFactory.Token(GetTypeSyntax(type)))));
			}
			return this;
		}

		public CodeTemplateBuilder SetParameters() {
			var parameterListNode = _template.GetAnnotatedNodes("ParameterList").First();
			_template = _template.ReplaceNode(parameterListNode, SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(_parameters)));
			return this;
		}

		public CodeTemplateBuilder UseParameterType(Type type, int parameterCount, bool isArray = false) {
			var parameterListNode = _template.GetAnnotatedNodes("ParameterList").First();
			var parameterList = new ParameterSyntax[parameterCount];
			for (int i = 0; i < parameterList.Length; i++) {
				if (isArray) {
					SyntaxFactory.Parameter(
						SyntaxFactory.Identifier(((char)('a' + i)).ToString()))
					.WithType(
						SyntaxFactory.ArrayType(
							SyntaxFactory.PredefinedType(
								SyntaxFactory.Token(GetTypeSyntax(type))))
						.WithRankSpecifiers(
							SyntaxFactory.SingletonList<ArrayRankSpecifierSyntax>(
								SyntaxFactory.ArrayRankSpecifier(
									SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
										SyntaxFactory.OmittedArraySizeExpression())))));
				} else {
					parameterList[i] = SyntaxFactory.Parameter(SyntaxFactory.Identifier(((char)('a' + i)).ToString()))
						.WithType(SyntaxFactory.PredefinedType(
							SyntaxFactory.Token(GetTypeSyntax(type))));
				}
			}
			_template = _template.ReplaceNode(parameterListNode, SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameterList)));
			return this;
		}

		public CodeTemplateBuilder UseReturnType(Type type) {
			SyntaxNode returnTypeNode;
			while ((returnTypeNode = _template.GetAnnotatedNodes("ReturnType").FirstOrDefault()) != null) {
				_template = _template.ReplaceNode(returnTypeNode, SyntaxFactory.PredefinedType(
					SyntaxFactory.Token(GetTypeSyntax(type))));
			}
			return this;
		}

		private SyntaxKind GetTypeSyntax(Type type) {
			if (type == typeof(bool)) {
				return SyntaxKind.BoolKeyword;
			}
			else if (type == typeof(int)) {
				return SyntaxKind.IntKeyword;
			}
			else if (type == typeof(float)) {
				return SyntaxKind.FloatKeyword;
			}
			else {
				return SyntaxKind.VoidKeyword;
			}
		}

		private CompilationUnitSyntax GetCodeTemplate() {
			return SyntaxFactory.CompilationUnit()
				.WithUsings(SyntaxFactory.List<UsingDirectiveSyntax>(
					new UsingDirectiveSyntax[]{
						SyntaxFactory.UsingDirective(
							SyntaxFactory.IdentifierName("System")),
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
										/*SyntaxFactory.TryStatement(
											SyntaxFactory.SingletonList<CatchClauseSyntax>(
												SyntaxFactory.CatchClause()
												.WithDeclaration(
													SyntaxFactory.CatchDeclaration(
														SyntaxFactory.IdentifierName("Exception"))
													.WithIdentifier(
														SyntaxFactory.Identifier("mainException")))))
										.WithBlock(*/
											SyntaxFactory.Block(
												SyntaxFactory.Block()
													.WithAdditionalAnnotations(new SyntaxAnnotation("SyntaxPlaceholder")))/*)*/,
										SyntaxFactory.ReturnStatement(
											SyntaxFactory.IdentifierName("ret"))))
										)
									)
								)
							)
				.NormalizeWhitespace();
		}

		public CompilationUnitSyntax Build() {
			return _template;
		}
	}
}
