using PatternGuidedGP.GP.Problems;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP {
	class DefaultAlgorithm : AlgorithmBase, IGenerationalAlgorithm {

		public DefaultAlgorithm(int populationSize, int generations) 
			: base(populationSize, generations) {
		}

		public override Individual Run(Problem problem) {
			Initializer.Initialize(Population, problem.RootType);
			Console.Write("Generation 0: ");
			Individual solution = EvaluatePopulation(problem);
			if (solution != null) {	// initial generation contains solution
				return solution;
			}
			for (int i = 0; i < Generations; i++) {
				Population = GetNextGeneration(Population);
				Console.Write("Generation {0}: ", (i + 1));
				solution = EvaluatePopulation(problem);
				if (solution != null) {
					return solution;
				}
			}
			Console.WriteLine("No solution found. Returning best:\n{0}", Population.GetFittest());
			return Population.GetFittest();
		}

		private Individual EvaluatePopulation(Problem problem) {
			problem.Evaluate(Population);
			Population.Sort();
			//Console.WriteLine("Best:\n{0}", Population.GetFittest());
			Console.WriteLine("Best fitness: {0}, Avg: {1}", Population.GetFittest().Fitness, Population.GetAverageFitness());

			if (IsSolutionFound()) {
				Console.WriteLine("Solution found:\n{0}", Population.GetFittest());
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
				if (RandomValueStore.Instance.GetDouble() < CrossoverRate) {
					child = Crossover.cross(Selector.Select(population), 
						Selector.Select(population));
				} else {
					child = new Individual(population.GetRandom());
				}
				if (RandomValueStore.Instance.GetDouble() < MutationRate) {
					Mutator.Mutate(child);
					child.FitnessEvaluated = false;
				}
				nextGen.Add(child);
			}
			return nextGen;
		}
	}
}
