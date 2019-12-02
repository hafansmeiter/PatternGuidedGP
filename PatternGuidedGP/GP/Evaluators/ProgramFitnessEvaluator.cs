using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.AbstractSyntaxTree;
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
using SyntaxTree = PatternGuidedGP.AbstractSyntaxTree.SyntaxTree;

namespace PatternGuidedGP.GP.Evaluators {
	class ProgramFitnessEvaluator : IFitnessEvaluator {

		public virtual IFitnessCalculator FitnessCalculator { get; set; }
		public ICompiler Compiler { get; set; } = new CSharpCompiler();	// default

		public bool AdjustCodeRequired { get; set; } = true;
		public bool AdjustLoopVariablesRequired { get; set; } = true;

		public virtual FitnessResult Evaluate(Individual individual, Problem problem) {
			TestSuite testSuite = problem.TestSuite;
			CompilationUnitSyntax compilationUnit = CreateCompilationUnit(individual, 
				testSuite.TestCases.First(),
				problem.CodeTemplate);
			Logger.WriteLine(4, "Run test method:");
			Logger.WriteLine(4, compilationUnit.NormalizeWhitespace().ToString());

			AppDomain appDomain = AppDomain.CreateDomain("AppDomain");
			var testable = GetTestableObject(appDomain, compilationUnit);

			PrepareTestRuns(individual, testSuite);

			int testCaseCount = testSuite.TestCases.Count;
			var results = new object[testSuite.TestCases.Count];
			for (int i = 0; i < testCaseCount; i++) {
				TestCase test = testSuite.TestCases[i];
				try {
					results[i] = RunTestCase(testable, test);
				} catch (Exception /*exception*/) {
					//Logger.WriteLine(4, "Exception: " + ex.GetType().Name);
					// Code does not run properly, e.g. DivideByZeroException
					// Count as negative run
				}
				OnTestRunFinished(individual, test, results[i]);
			}

			AppDomain.Unload(appDomain);

			FitnessResult fitness = FitnessCalculator.CalculateFitness(individual, testSuite, results);
			OnEvaluationFinished(individual, fitness, results);
			return fitness;
		}

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
			var syntaxTree = individual.SyntaxTree;
			if (AdjustCodeRequired) {
				AdjustCode(syntaxTree);
			}
			var syntax = syntaxTree.GetSyntaxNode();
			return CreateCompilationUnit(syntax, sample, template);
		}

		private void AdjustCode(SyntaxTree tree) {
			if (AdjustLoopVariablesRequired) {
				AdjustLoopVariables(tree);
			}
		}

		private void AdjustLoopVariables(SyntaxTree tree) {
			foreach (var node in tree.RootNodes) {
				AdjustLoopVariables(node, "");
			}
		}

		private void AdjustLoopVariables(TreeNode node, string loopVariableName) {
			if (node is ForLoopTimesStatement) {
				var forStatement = node as ForLoopTimesStatement;
				loopVariableName = forStatement.LoopVariableName;
			}
			else if (node is ForLoopVariable) {
				var loopVariable = node as ForLoopVariable;
				loopVariable.Name = loopVariableName;
			}
			foreach (var child in node.Children) {
				AdjustLoopVariables(child, loopVariableName);
			}
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
