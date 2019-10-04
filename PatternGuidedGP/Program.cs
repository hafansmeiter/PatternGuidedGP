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

			Problem problem = new AllEqualProblem(3);
			problem.FitnessEvaluator = evaluator;

			var generator = new KozaTreeGeneratorGrow();
			generator.TreeNodeRepository = problem.TreeNodeRepository;

			DefaultAlgorithm algorithm = new DefaultAlgorithm(populationSize: 100, generations: 20);
			algorithm.Crossover = new RandomSubtreeCrossover(maxTreeDepth: 5);
			algorithm.CrossoverRate = 0.8;
			algorithm.Elitism = 5;
			algorithm.Initializer = new RampedHalfHalfInitializer(problem.TreeNodeRepository);
			algorithm.MaxTreeDepth = 5;
			algorithm.MutationRate = 0.2;
			algorithm.Mutator = new RandomSubtreeMutator(generator, maxTreeDepth: 5, maxMutationTreeDepth: 3);
			algorithm.Selector = new TournamentSelector(7);

			Individual individual = algorithm.Run(problem);
			Console.WriteLine("Result solution: " + individual);

			Console.ReadKey();
		}
	}
}
