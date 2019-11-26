using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.Compiler;
using PatternGuidedGP.Compiler.CSharp;
using PatternGuidedGP.GP.Problems;
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Evaluators {
	abstract class ProgramFitnessEvaluator : IFitnessEvaluator {

		protected class FitnessResult {
			public double Fitness { get; private set; }

			public FitnessResult(double fitness) {
				Fitness = fitness;
			}
		}

		public ICompiler Compiler { get; set; } = new CSharpCompiler();	// default

		public virtual double Evaluate(Individual individual, Problem problem) {
			TestSuite testSuite = problem.TestSuite;
			CompilationUnitSyntax compilationUnit = CreateCompilationUnit(individual, 
				testSuite.TestCases.First(),
				problem.CodeTemplate);
			Logger.WriteLine(4, "Run test method:");
			Logger.WriteLine(4, compilationUnit.NormalizeWhitespace().ToString());

			//AppDomain appDomain = null;
			AppDomain appDomain = AppDomain.CreateDomain("AppDomain");
			var testable = GetTestableObject(appDomain, compilationUnit);

			PrepareTestRuns(individual, testSuite);

			int testCaseCount = testSuite.TestCases.Count;
			var results = new object[testSuite.TestCases.Count];
			for (int i = 0; i < testCaseCount; i++) {
				TestCase test = testSuite.TestCases[i];
				try {
					results[i] = RunTestCase(testable, test);
				} catch (Exception ex) {
					//Logger.WriteLine(4, "Exception: " + ex.GetType().Name);
					// Code does not run properly, e.g. DivideByZeroException
					// Count as negative run
				}
				OnTestRunFinished(individual, test, results[i]);
			}

			AppDomain.Unload(appDomain);

			FitnessResult fitness = CalculateFitness(individual, testSuite, results);
			OnEvaluationFinished(individual, fitness, results);
			return fitness.Fitness;
		}

		protected abstract FitnessResult CalculateFitness(Individual individual, TestSuite testSuite, object[] results);

		protected virtual void PrepareTestRuns(Individual individual, TestSuite testSuite) {
		}

		protected virtual void OnTestRunFinished(Individual individual, TestCase testCase, object result) {
		}

		protected virtual void OnEvaluationFinished(Individual individual, FitnessResult fitness, object [] results) {
		}

		protected virtual object RunTestCase(ITestable testable, TestCase test) {
			return testable.RunTest(test.Parameter);
		}

		protected virtual CompilationUnitSyntax CreateCompilationUnit(Individual individual, TestCase sample, CompilationUnitSyntax template) {
			var syntax = individual.SyntaxTree.Root.GetSyntaxNode();
			return CreateCompilationUnit(syntax, sample, template);
		}

		protected virtual CompilationUnitSyntax CreateCompilationUnit(SyntaxNode syntax, TestCase sample, CompilationUnitSyntax template) {
			var newSyntax = ReplacePlaceholder(template, syntax);
			// replacing types moved to CodeTemplateBuilder
			//newSyntax = ReplaceParameterList(newSyntax, sample);
			//newSyntax = ReplaceReturnType(newSyntax, sample);
			return newSyntax.NormalizeWhitespace();
		}

		private ITestable GetTestableObject(AppDomain appDomain, CompilationUnitSyntax compilationUnit) {
			return Compiler.Compile(appDomain, compilationUnit);
		}

		private CompilationUnitSyntax ReplacePlaceholder(CompilationUnitSyntax template, SyntaxNode syntax) {
			var returnValueNode = template.GetAnnotatedNodes("SyntaxPlaceholder").First();
			return template.ReplaceNode(returnValueNode, syntax);
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
	}
}
