using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.Compiler;
using PatternGuidedGP.GP.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP {
	class ProgramFitnessEvaluator : IFitnessEvaluator {

		public ICompiler Compiler { get; set; }
		public CompilationUnitSyntax Template { get; set; }
		public SyntaxList<AttributeListSyntax> SyntaxIdentifier { get; private set; }

		public double Evaluate(Individual individual, TestSuite testSuite) {
			CompilationUnitSyntax compilationUnit = CreateCompilationUnit(individual.Syntax, testSuite.TestCases.First());
			//Console.WriteLine("Run test method:");
			//Console.WriteLine(compilationUnit.ToString());
			MethodInfo method = GetTestMethod(compilationUnit);

			int positive = 0;
			foreach (TestCase test in testSuite.TestCases) {
				object result = method.Invoke(null, test.Parameter);

				//Console.WriteLine("Test case: " + test.ToString() + ", GP result=" + result);
				if (result.Equals(test.Result)) {
					positive++;
				}
			}
			return 1 - ((double) positive / (double) testSuite.TestCases.Count);
		}

		private MethodInfo GetTestMethod(CompilationUnitSyntax compilationUnit) {
			var assembly = Compiler.Compile(compilationUnit);
			Type t = assembly.GetType("ProblemClass");
			MethodInfo method = t.GetMethod("Test");
			return method;
		}

		private CompilationUnitSyntax CreateCompilationUnit(SyntaxNode syntax, TestCase testCase) {
			var newSyntax = ReplaceReturnValue(Template, syntax);
			newSyntax = ReplaceParameterList(newSyntax, testCase);
			newSyntax = ReplaceReturnType(newSyntax, testCase).NormalizeWhitespace();
			return newSyntax;
		}

		private CompilationUnitSyntax ReplaceReturnValue(CompilationUnitSyntax template, SyntaxNode syntax) {
			var returnValueNode = template.GetAnnotatedNodes("ReturnValue").First();
			return Template.ReplaceNode(returnValueNode, syntax);
		}

		private CompilationUnitSyntax ReplaceParameterList(CompilationUnitSyntax template, TestCase test) {
			var parameterListNode = template.GetAnnotatedNodes("ParameterList").First();
			var parameterList = new ParameterSyntax[test.Parameter.Length];
			for (int i = 0; i < parameterList.Length; i++) {
				parameterList[i] = SyntaxFactory.Parameter(SyntaxFactory.Identifier(((char)('a' + i)).ToString()))
					.WithType(SyntaxFactory.PredefinedType(
						SyntaxFactory.Token(test.Parameter[i].GetType() == typeof(bool) ? SyntaxKind.BoolKeyword : SyntaxKind.IntKeyword)));
			}
			return template.ReplaceNode(parameterListNode, SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameterList)));
		}

		private CompilationUnitSyntax ReplaceReturnType(CompilationUnitSyntax template, TestCase test) {
			var returnTypeNode = template.GetAnnotatedNodes("ReturnType").First();
			return template.ReplaceNode(returnTypeNode, SyntaxFactory.PredefinedType(
				SyntaxFactory.Token(test.Result.GetType() == typeof(bool) ? SyntaxKind.BoolKeyword : SyntaxKind.IntKeyword)));
		}
	}
}
