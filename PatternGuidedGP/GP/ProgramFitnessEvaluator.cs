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

		public double Evaluate(Individual individual, TestSuite testSuite, CompilationUnitSyntax template) {
			CompilationUnitSyntax compilationUnit = CreateCompilationUnit(individual.Syntax, 
				testSuite.TestCases.First(),
				template);
			//Console.WriteLine("Run test method:");
			//Console.WriteLine(compilationUnit.ToString());
			MethodInfo method = GetTestMethod(compilationUnit);

			int positive = 0;
			foreach (TestCase test in testSuite.TestCases) {
				object result;
				try {
					result = method.Invoke(null, test.Parameter);

					//Console.WriteLine("Test case: " + test.ToString() + ", GP result=" + result);
					if (result.Equals(test.Result)) {
						positive++;
					}
				} catch (Exception e) {
					// Code does not run properly, e.g. DivideByZeroException
					// Count as negative run
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

		private CompilationUnitSyntax CreateCompilationUnit(SyntaxNode syntax, TestCase testCase, CompilationUnitSyntax template) {
			var newSyntax = ReplacePlaceholder(template, syntax);
			newSyntax = ReplaceParameterList(newSyntax, testCase);
			newSyntax = ReplaceReturnType(newSyntax, testCase);
			return newSyntax.NormalizeWhitespace();
		}

		private CompilationUnitSyntax ReplacePlaceholder(CompilationUnitSyntax template, SyntaxNode syntax) {
			var returnValueNode = template.GetAnnotatedNodes("SyntaxPlaceholder").First();
			return template.ReplaceNode(returnValueNode, syntax);
		}

		private CompilationUnitSyntax ReplaceParameterList(CompilationUnitSyntax template, TestCase test) {
			var parameterListNode = template.GetAnnotatedNodes("ParameterList").First();
			var parameterList = new ParameterSyntax[test.Parameter.Length];
			for (int i = 0; i < parameterList.Length; i++) {
				parameterList[i] = SyntaxFactory.Parameter(SyntaxFactory.Identifier(((char)('a' + i)).ToString()))
					.WithType(SyntaxFactory.PredefinedType(
						SyntaxFactory.Token(GetTypeSyntax(test.Parameter[i].GetType()))));
			}
			return template.ReplaceNode(parameterListNode, SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameterList)));
		}

		private CompilationUnitSyntax ReplaceReturnType(CompilationUnitSyntax template, TestCase test) {
			SyntaxNode returnTypeNode;
			while ((returnTypeNode = template.GetAnnotatedNodes("ReturnType").FirstOrDefault()) != null) {
				template = template.ReplaceNode(returnTypeNode, SyntaxFactory.PredefinedType(
					SyntaxFactory.Token(GetTypeSyntax(test.Result.GetType()))));
			}
			return template;
		}

		/*private CompilationUnitSyntax ReplaceReturnValue(CompilationUnitSyntax template, TestCase test) {
			SyntaxNode returnValueNode;
			while ((returnValueNode = template.GetAnnotatedNodes("InitReturnValue").FirstOrDefault()) != null) {
				template = template.ReplaceNode(returnValueNode,
					SyntaxFactory.DefaultExpression(SyntaxFactory.PredefinedType(
													SyntaxFactory.Token(GetTypeSyntax(test.Result.GetType())))));
			}
			return template;
		}*/

		private object GetDefaultValue(Type type) {
			if (type.IsValueType) {
				return Activator.CreateInstance(type);
			}
			return null;
		}

		private SyntaxKind GetTypeSyntax(Type type) {
			if (type == typeof(bool)) {
				return SyntaxKind.BoolKeyword;
			} else if (type == typeof(int)) {
				return SyntaxKind.IntKeyword;
			} else {
				return SyntaxKind.VoidKeyword;
			}
		}
	}
}
