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
			Logger.WriteLine(1, "No solution found.");
			Logger.WriteLine(2, string.Format("Returning best:\n{0}", Population.GetFittest()));
			return Population.GetFittest();
		}

		private Individual EvaluatePopulation(Problem problem) {
			problem.Evaluate(Population);
			Population.Sort();
			//Console.WriteLine("Best:\n{0}", Population.GetFittest());
			Logger.WriteLine(1, string.Format("Best fitness: {0}, Avg: {1}", Population.GetFittest().Fitness, Population.GetAverageFitness()));

			if (IsSolutionFound()) {
				Logger.WriteLine(1, "Solution found.");
				Logger.WriteLine(2, string.Format("Solution:\n{0}", Population.GetFittest()));
				return Population.GetFittest();
			}
			return null;
		}

		public override bool IsSolutionFound() {
			// assume evaluated and sorted individuals
			return Population.GetFittest().Fitness == 0;
		}

		public override Population GetNextGeneration(Population population) {
			int size = population.Size;
			Population nextGen = new Population(size);
			nextGen.Add(population.GetFittest(Elitism).ToArray());
			int duplicates = 0;
			while (nextGen.IndividualCount < size) {
				IList<Individual> children = new List<Individual>();
				// create child by crossover or copy from old population
				if (RandomValueGenerator.Instance.GetDouble() < CrossoverRate) {
					foreach (var child in Crossover.cross(Selector.Select(population), Selector.Select(population))) {
						children.Add(new Individual(child));
					}
				}
				else {
					children.Add(new Individual(population.GetRandom()));
				}
				foreach (var child in children) {
					if (RandomValueGenerator.Instance.GetDouble() < MutationRate) {
						if (Mutator.Mutate(child)) {
							child.FitnessEvaluated = false;
						}
					}
					if (AllowDuplicates || !nextGen.ContainsIndividual(child)) {
						nextGen.Add(child);
					} else {
						duplicates++;
					}
				}
			}
			if (!AllowDuplicates) {
				Logger.WriteLine(1, "Generated duplicates: " + duplicates);
			}
			return nextGen;
		}
	}
}
