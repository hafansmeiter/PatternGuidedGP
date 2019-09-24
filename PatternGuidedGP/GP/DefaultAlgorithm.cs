using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP {
	class DefaultAlgorithm : AlgorithmBase, IGenerationalAlgorithm {

		private Random _random = new Random();

		public DefaultAlgorithm(Problem problem, int populationSize, int generations) 
			: base(problem, populationSize, generations) {
		}

		public override Individual Run() {
			Initializer.Initialize(Population, MaxTreeDepth);
			Console.WriteLine("Generation 0:");
			Individual solution = EvaluatePopulation();
			if (solution != null) {	// initial generation contains solution
				return solution;
			}
			for (int i = 0; i < Generations; i++) {
				Population = GetNextGeneration(Population);
				Console.WriteLine("Generation {0}:", (i + 1));
				solution = EvaluatePopulation();
				if (solution != null) {
					return solution;
				}
			}
			Console.WriteLine("No solution found. Returning best={0}", Population.GetFittest());
			return Population.GetFittest();
		}

		private Individual EvaluatePopulation() {
			Problem.Evaluate(Population);
			Population.Sort();
			Console.WriteLine("Best={0}, Fitness={1}, Avg={2}", Population.GetFittest(), Population.GetFittest().Fitness, Population.GetAverageFitness());

			if (IsSolutionFound()) {
				Console.WriteLine("Solution found: {0}", Population.GetFittest());
				return Population.GetFittest();
			}
			return null;
		}

		public override bool IsSolutionFound() {
			// assume evaluated and sorted individuals
			return Population.GetFittest().Fitness == 0;
		}

		public override Population GetNextGeneration(Population population) {
			Population nextGen = new Population(population.Size);
			nextGen.Add(population.GetFittest(Elitism).ToArray());
			for (int i = 0; i < nextGen.Size - Elitism; i++) {
				Individual child = null;
				// create child by crossover or copy from old population
				if (_random.NextDouble() < CrossoverRate) {
					child = Crossover.cross(Selector.Select(population), 
						Selector.Select(population));
				} else {
					child = new Individual(Selector.Select(population));
				}
				if (_random.NextDouble() < MutationRate) {
					Mutator.Mutate(child);
					child.FitnessEvaluated = false;
				}
				nextGen.Add(child);
			}
			return nextGen;
		}
	}
}
