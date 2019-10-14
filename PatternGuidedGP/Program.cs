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
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.Compiler.CSharp;
using PatternGuidedGP.GP;
using PatternGuidedGP.GP.Operators;
using PatternGuidedGP.GP.Problems;
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Pangea;
using PatternGuidedGP.Util;

namespace PatternGuidedGP {
	class Program {
		static void Main(string[] args) {
			Logger.Level = 3;

			var compiler = new CSharpCompiler();
			var defaultEvaluator = new DefaultFitnessEvaluator() {
				Compiler = compiler
			};
			var mdlEvaluator = new MDLFitnessEvaluator() {
				Compiler = compiler
			};

			var configurations = new[] {
				new RunConfiguration("Standard PANGEA") { FitnessEvaluator = mdlEvaluator },
				new RunConfiguration("Standard GP") { FitnessEvaluator = defaultEvaluator }
			};

			Problem[] problems = new Problem[] {
				new AllEqualProblem(3),
				new ContainsFirstProblem(3),
				new CountZeroesProblem(3),
				new IsOrderedProblem(3),
				new MajorityProblem(3),
				new MaximumProblem(3)
			};

			foreach (var config in configurations) {
				Logger.WriteLine(1, "Run configuration: " + config.Name);
				foreach (var problem in problems) {
					Logger.WriteLine(1, problem.GetType().Name + ":");
					problem.FitnessEvaluator = config.FitnessEvaluator;

					var generator = new KozaTreeGeneratorGrow();
					generator.TreeNodeRepository = problem.TreeNodeRepository;

					DefaultAlgorithm algorithm = new DefaultAlgorithm(populationSize: 300, generations: 300);
					algorithm.Crossover = new RandomSubtreeCrossover(maxTreeDepth: 7);
					algorithm.CrossoverRate = 0.7;
					algorithm.Elitism = 5;
					algorithm.Initializer = new RampedHalfHalfInitializer(maxTreeDepth: 7, repository: problem.TreeNodeRepository);
					algorithm.MaxTreeDepth = 9;
					algorithm.MutationRate = 0.2;
					algorithm.Mutator = new RandomSubtreeMutator(generator, maxTreeDepth: 7, maxMutationTreeDepth: 3);
					algorithm.Selector = new TournamentSelector(7);

					int solved = 0;
					for (int i = 0; i < config.Runs; i++) {
						Individual individual = algorithm.Run(problem);
						Logger.WriteLine(1, "Result solution:\n" + individual);
						if (individual != null) {
							solved++;
						}
					}
					Logger.WriteLine(1, problem.GetType().Name + ": Solved " + solved + "/" + config.Runs);
					Logger.WriteLine(1, "=====================================================");
				}
			}

			Console.ReadKey();
		}
	}
}
