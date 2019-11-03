using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math.Optimization.Losses;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.Pool;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.Compiler.CSharp;
using PatternGuidedGP.GP;
using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.GP.Operators;
using PatternGuidedGP.GP.Problems;
using PatternGuidedGP.GP.SemanticGP;
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Pangea;
using PatternGuidedGP.Util;

namespace PatternGuidedGP {
	class Program {
		
		static void Main(string[] args) {
			int runConfig, runProblem;
			EvaluateArgs(args, out runConfig, out runProblem);

			Logger.Level = 1;

			int maxTreeDepth = 5;
			int maxMutationTreeDepth = 3;
			int populationSize = 50;
			int generations = 20;

			var semanticsBasedSubTreePool = new SemanticsBasedSubTreePool();
			var generator = new KozaTreeGeneratorGrow();

			var configurations = new[] {
				new RunConfiguration("Standard GP") {
					FitnessEvaluator = new DefaultFitnessEvaluator(),
					Crossover = new RandomSubtreeCrossover(maxTreeDepth),
					Mutator = new RandomSubtreeMutator(generator, maxTreeDepth, maxMutationTreeDepth)
				},
				new RunConfiguration("Standard PANGEA") {
					FitnessEvaluator = new MDLFitnessEvaluator(),
					Crossover = new RandomSubtreeCrossover(maxTreeDepth),
					Mutator = new RandomSubtreeMutator(generator, maxTreeDepth, maxMutationTreeDepth)
				},
				new RunConfiguration("PANGEA + Semantic Backpropagation Operators") {
					FitnessEvaluator = new SemanticMDLFitnessEvaluator() {
						SubTreePool = semanticsBasedSubTreePool
					},
					Crossover = new ApproximatelyGeometricSemanticCrossover(semanticsBasedSubTreePool, maxTreeDepth),
					Mutator = new ApproximatelySemanticMutator(semanticsBasedSubTreePool, maxTreeDepth)
				}
			};

			Problem[] problems = new Problem[] {
				new AllEqualProblem(3),
				new ContainsFirstProblem(3),
				new CountZeroesProblem(3),
				new IsOrderedProblem(3),
				new MajorityProblem(3),
				new MaximumProblem(3),
				new CompareProblem(6),
				new MultiplexerProblem(6, 2),
				new ParityProblem(6)
			};

			if (runConfig >= 0) {
				configurations = new[] { configurations[runConfig] };
			}
			if (runProblem >= 0) {
				problems = new[] { problems[runProblem] };
			}

			foreach (var config in configurations) {
				Logger.FileName = "..\\..\\..\\" + config.Name + ".txt";
				Logger.WriteLine(0, "Run configuration: " + config.Name);
				foreach (var problem in problems) {
					Logger.WriteLine(0, problem.GetType().Name + ":");
					problem.FitnessEvaluator = config.FitnessEvaluator;
					generator.TreeNodeRepository = problem.TreeNodeRepository;
					if (config.Crossover is IGeometricOperator) {
						((IGeometricOperator)config.Crossover).GeometricCalculator = problem.GeometricCalculator;
					}
					if (config.Mutator is ISemanticOperator) {
						((ISemanticOperator)config.Mutator).DesiredSemantics = problem.TestSuite.Semantics;
					}

					int solved = 0;
					for (int i = 0; i < config.Runs; i++) {
						DefaultAlgorithm algorithm = new DefaultAlgorithm(populationSize, generations) {
							Crossover = config.Crossover,
							CrossoverRate = 0.7,
							Elitism = 5,
							Initializer = new RampedHalfHalfInitializer(maxTreeDepth, problem.TreeNodeRepository),
							MaxTreeDepth = 9,	// not used
							MutationRate = 0.2,
							Mutator = config.Mutator,
							Selector = new TournamentSelector(7)
						};

						Individual bestSolution = algorithm.Run(problem);
						if (algorithm.IsSolutionFound()) {
							solved++;
						}
					}
					Logger.WriteLine(0, problem.GetType().Name + ": Solved " + solved + "/" + config.Runs);
					Logger.WriteLine(0, "=====================================================");
				}
			}
		}

		private static void EvaluateArgs(string [] args, out int runConfig, out int runProblem) {
			runConfig = -1;
			runProblem = -1;
			foreach (var arg in args) {
				if (arg.StartsWith("/config:")) {
					runConfig = int.Parse(arg.Substring(8));
				} else if (arg.StartsWith("/problem:")) {
					runProblem = int.Parse(arg.Substring(9));
				}
			}
		}
	}
}
