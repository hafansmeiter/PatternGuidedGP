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
	abstract class Problem {
		public IFitnessEvaluator FitnessEvaluator { get; set; }
		public TestSuite TestSuite { get; }
		public CompilationUnitSyntax CodeTemplate { get; }
		public TreeNodeRepository TreeNodeRepository { get; } = new TreeNodeRepository();
		public abstract Type RootType { get; }
		public abstract Type ReturnType { get; }
		public abstract Type ParameterType { get; }
		public int ParameterCount { get; set; }

		public Problem(int n) {
			ParameterCount = n;
			TestSuite = GetTestSuite();
			CodeTemplate = GetCodeTemplate();
			AddTreeNodes(TreeNodeRepository);

			Console.WriteLine(GetType().Name + " test suite:");
			foreach (var test in TestSuite.TestCases) {
				foreach (var param in test.Parameter) {
					Console.Write(param + " ");
				}
				Console.WriteLine(test.Result);
			}
		}

		protected virtual void AddTreeNodes(TreeNodeRepository repository) {
			if (ParameterType == typeof(int)) {
				AddIntParameters(repository);
			} else if (ParameterType == typeof(bool)) {
				AddBoolParameters(repository);
			}
		}

		protected void AddIntParameters(TreeNodeRepository repository) {
			for (int i = 0; i < ParameterCount; i++) {
				repository.Add(new IntIdentifierExpression(((char)('a' + i)).ToString()));
			}
		}

		protected void AddBoolParameters(TreeNodeRepository repository) {
			for (int i = 0; i < ParameterCount; i++) {
				repository.Add(new BoolIdentifierExpression(((char)('a' + i)).ToString()));
			}
		}

		protected abstract TestSuite GetTestSuite();
		protected abstract CompilationUnitSyntax GetCodeTemplate();

		public void Evaluate(Population population) {
			foreach (var individual in population.Individuals) {
				if (!individual.FitnessEvaluated) {
					double fitness = FitnessEvaluator.Evaluate(individual, TestSuite, CodeTemplate);
					individual.Fitness = fitness;
				}
			}
		}
	}
}
