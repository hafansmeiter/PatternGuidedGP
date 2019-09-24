using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.Compiler;
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

		public double Evaluate(Individual individual, TestSuite testSuite) {
			CompilationUnitSyntax compilationUnit = CreateCompilationUnit(individual.Syntax);
			MethodInfo method = GetTestMethod(compilationUnit);

			//Console.WriteLine("Run test method:");
			//Console.WriteLine(compilationUnit.ToString());

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

		private CompilationUnitSyntax CreateCompilationUnit(SyntaxNode syntax) {
			var returnValueNode = Template.GetAnnotatedNodes("ReturnValue").First();
			var newSyntax = Template.ReplaceNode(returnValueNode, syntax);
			return newSyntax;
		}
	}
}
