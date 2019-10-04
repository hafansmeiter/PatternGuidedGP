using Microsoft.CodeAnalysis.CSharp.Syntax;
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
		public TreeNodeRepository TreeNodeRepository { get; }
		public abstract Type RootType { get; }

		protected int _parameterCount;

		public Problem(int n) {
			_parameterCount = n;
			TestSuite = GetTestSuite();
			CodeTemplate = GetCodeTemplate();
			TreeNodeRepository = new TreeNodeRepository();
			AddTreeNodes(TreeNodeRepository);
		}

		protected abstract TestSuite GetTestSuite();
		protected abstract CompilationUnitSyntax GetCodeTemplate();
		protected abstract void AddTreeNodes(TreeNodeRepository repository);

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
