using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

namespace PatternGuidedGP {
	class Program {
		static void Main(string[] args) {
			var compiler = new CSharpCompiler();
			var evaluator = new ProgramFitnessEvaluator();
			evaluator.Compiler = compiler;

			Problem[] problems = new Problem[] {
				//new AllEqualProblem(3),
				//new ContainsFirstProblem(3),
				new CountZeroesProblem(3),
				new IsOrderedProblem(3),
				new MajorityProblem(3),
				new MaximumProblem(3)
			};

			foreach (var problem in problems) {
				Console.WriteLine(problem.GetType().Name + ":");
				problem.FitnessEvaluator = evaluator;

				var generator = new KozaTreeGeneratorGrow();
				generator.TreeNodeRepository = problem.TreeNodeRepository;

				DefaultAlgorithm algorithm = new DefaultAlgorithm(populationSize: 300, generations: 300);
				algorithm.Crossover = new RandomSubtreeCrossover(maxTreeDepth: 9);
				algorithm.CrossoverRate = 0.7;
				algorithm.Elitism = 5;
				algorithm.Initializer = new RampedHalfHalfInitializer(maxTreeDepth: 3, repository: problem.TreeNodeRepository);
				algorithm.MaxTreeDepth = 9;
				algorithm.MutationRate = 0.2;
				algorithm.Mutator = new RandomSubtreeMutator(generator, maxTreeDepth: 9, maxMutationTreeDepth: 3);
				algorithm.Selector = new TournamentSelector(7);

				int solved = 0;
				for (int i = 0; i < 20; i++) {
					Individual individual = algorithm.Run(problem);
					Console.WriteLine("Result solution:\n" + individual);
					if (individual != null) {
						solved++;
					}
				}
				Console.WriteLine(problem.GetType().Name + ": Solved " + solved + "/20");
				Console.WriteLine("=====================================================");
			}

			Console.ReadKey();
		}
	}
}
