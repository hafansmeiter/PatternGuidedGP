using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.GP.SemanticGP;
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Problems {
	abstract class Problem {
		public IFitnessEvaluator FitnessEvaluator { get; set; }
		public IGeometricCalculator GeometricCalculator { get; set; }
		public TestSuite TestSuite { get; protected set; }
		public CompilationUnitSyntax CodeTemplate { get; protected set; }
		public TreeNodeRepository TreeNodeRepository { get; } = new TreeNodeRepository();
		public abstract Type RootType { get; }
		public abstract Type ReturnType { get; }
		public abstract Type ParameterType { get; }
		public int ParameterCount { get; set; }
		
		public Problem(int n, bool initialize = true) {
			ParameterCount = n;
			if (initialize) {
				Initialize();
			}
		}

		protected void Initialize() {
			TestSuite = GetTestSuite();
			CodeTemplate = GetCodeTemplate();
			GeometricCalculator = GetGeometricCalculator();
			AddTreeNodes(TreeNodeRepository);

			Logger.WriteLine(4, GetType().Name + " test suite:");
			foreach (var test in TestSuite.TestCases) {
				foreach (var param in test.Parameter) {
					Logger.Write(4, param + " ");
				}
				Logger.WriteLine(4, "-> " + test.Result.ToString());
			}
		}

		protected virtual IGeometricCalculator GetGeometricCalculator() {
			if (ReturnType == typeof(bool)) {
				return new BoolGeometricCalculator();
			} else if (ReturnType == typeof(int)) {
				return new IntGeometricCalculator();
			}
			return null;
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

		public int Evaluate(Population population) {
			int evaluationCount = 0;
			foreach (var individual in population.Individuals) {
				Logger.WriteLine(4, "Individual tree height: " + individual.SyntaxTree.Height);
				if (!individual.FitnessEvaluated) {
					double fitness = FitnessEvaluator.Evaluate(individual, this);
					individual.Fitness = fitness;
					evaluationCount++;
				}
			}
			return evaluationCount;
		}
	}
}
