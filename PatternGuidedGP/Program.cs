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
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Pangea;
using PatternGuidedGP.Util;

namespace PatternGuidedGP {
	class Program {
		
		static void Main(string[] args) {
			Logger.Level = 0;

			var subTreePool = new FitnessBasedSubTreePool();

			var compiler = new CSharpCompiler();
			var defaultEvaluator = new DefaultFitnessEvaluator() {
				Compiler = compiler
			};
			var mdlEvaluator = new MDLFitnessEvaluator() {
				Compiler = compiler,
				//SubTreePool = subTreePool
			};

			var configurations = new[] {
				new RunConfiguration("Standard PANGEA") { FitnessEvaluator = mdlEvaluator },
				new RunConfiguration("Standard GP") { FitnessEvaluator = defaultEvaluator },
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

			foreach (var config in configurations) {
				Logger.WriteLine(0, "Run configuration: " + config.Name);
				foreach (var problem in problems) {
					Logger.WriteLine(0, problem.GetType().Name + ":");
					problem.FitnessEvaluator = config.FitnessEvaluator;

					var generator = new KozaTreeGeneratorGrow();
					generator.TreeNodeRepository = problem.TreeNodeRepository;

					int solved = 0;
					for (int i = 0; i < config.Runs; i++) {
						DefaultAlgorithm algorithm = new DefaultAlgorithm(populationSize: 100, generations: 50) {
							Crossover = new RandomSubtreeCrossover(maxTreeDepth: 7),
							CrossoverRate = 0.7,
							Elitism = 5,
							Initializer = new RampedHalfHalfInitializer(maxTreeDepth: 7, repository: problem.TreeNodeRepository),
							MaxTreeDepth = 9,
							MutationRate = 0.2,
							Mutator = new RandomSubtreeMutator(generator, maxTreeDepth: 7, maxMutationTreeDepth: 3),
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

			Console.ReadKey();
		}
	}
}
