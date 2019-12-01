﻿using PatternGuidedGP.GP.Problems;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP {
	class DefaultAlgorithm : AlgorithmBase {

		public DefaultAlgorithm(int populationSize, int generations, bool allowDuplicates)
			: base(populationSize, generations, allowDuplicates) {
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
				if (SubTreePool != null) {
					SubTreePool.GenerationFinished();
				}
			}
			Logger.WriteLine(1, "No solution found.");
			Logger.WriteLine(2, string.Format("Returning best:\n{0}", Population.GetFittest()));
			return Population.GetFittest();
		}

		private Individual EvaluatePopulation(Problem problem) {
			int evaluationCount = problem.Evaluate(Population);
			Population.Sort();
			//Console.WriteLine("Best:\n{0}", Population.GetFittest());
			Logger.WriteLine(1, string.Format("Evaluated " + evaluationCount + "/" + Population.Size + " individuals"));
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
			Population nextGen = new Population(population.Size, population.AllowDuplicates);
			nextGen.Add(population.GetFittest(Elitism).ToArray());
			int duplicates = 0;
			while (!nextGen.IsFull) {
				IList<Individual> children = new List<Individual>();
				// create child by crossover or copy from old population
				if (RandomValueGenerator.Instance.GetDouble() < CrossoverRate) {
					var individual1 = Selector.Select(population);
					var individual2 = Selector.Select(population);
					foreach (var child in Crossover.cross(individual1, individual2)) {
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
					int added = nextGen.Add(child);
					duplicates += 1 - added;
				}
			}
			if (!AllowDuplicates) {
				Logger.WriteLine(2, "Generated duplicates: " + duplicates);
			}
			return nextGen;
		}
	}
}
