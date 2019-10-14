using PatternGuidedGP.GP.Problems;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP {
	class DefaultAlgorithm : AlgorithmBase {

		public DefaultAlgorithm(int populationSize, int generations)
			: base(populationSize, generations) {
		}

		public override Individual Run(Problem problem) {
			Initializer.Initialize(Population, problem.RootType);
			Logger.WriteLine(1, "Generation 0: ");
			Individual solution = EvaluatePopulation(problem);
			if (solution != null) { // initial generation contains solution
				return solution;
			}
			for (int i = 0; i < Generations; i++) {
				Population = GetNextGeneration(Population);
				Logger.WriteLine(1, string.Format("Generation {0}: ", (i + 1)));
				solution = EvaluatePopulation(problem);
				if (solution != null) {
					return solution;
				}
			}
			Logger.WriteLine(1, string.Format("No solution found. Returning best:\n{0}", Population.GetFittest()));
			return Population.GetFittest();
		}

		private Individual EvaluatePopulation(Problem problem) {
			problem.Evaluate(Population);
			Population.Sort();
			//Console.WriteLine("Best:\n{0}", Population.GetFittest());
			Logger.WriteLine(1, string.Format("Best fitness: {0}, Avg: {1}", Population.GetFittest().Fitness, Population.GetAverageFitness()));

			if (IsSolutionFound()) {
				Logger.WriteLine(1, string.Format("Solution found:\n{0}", Population.GetFittest()));
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
					child = new Individual(Crossover.cross(Selector.Select(population),
						Selector.Select(population)));
				}
				else {
					child = new Individual(population.GetRandom());
				}
				if (RandomValueStore.Instance.GetDouble() < MutationRate) {
					if (Mutator.Mutate(child)) {
						child.FitnessEvaluated = false;
					}
				}
				nextGen.Add(child);
			}
			return nextGen;
		}
	}
}
