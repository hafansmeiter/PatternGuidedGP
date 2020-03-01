using PatternGuidedGP.GP.Problems;
using PatternGuidedGP.Pangea;
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
			var notNullParents = Population.Individuals.Where(ind => ind.SyntaxTree.RootNodes[0].Parent != null);
			if (notNullParents.Count() > 0) {
				Console.WriteLine("NOT NULL PARENT AFTER INITIALIZE");
			}
			Logger.WriteLine(1, "Generation 0: ");
			// .csv header
			Logger.WriteLine(0, "Generation;Best_fitness;Best_program_error;Best_classification_error;Best_tree_size;Avg_fitness;Evaluated;Dist_to_optimal");
			Individual solution = EvaluatePopulation(problem, 0);
			if (solution != null) { // initial generation contains solution
				return solution;
			}
			for (int i = 0; i < Generations; i++) {
				Population = GetNextGeneration(Population);
				Logger.WriteLine(1, string.Format("Generation {0}: ", (i + 1)));
				solution = EvaluatePopulation(problem, i + 1);
				if (solution != null) {
					return solution;
				}
			}
			Logger.WriteLine(1, "No solution found.");
			Logger.WriteLine(2, string.Format("Returning best:\n{0}", Population.GetFittest()));
			return Population.GetFittest();
		}

		private Individual EvaluatePopulation(Problem problem, int generation) {
			int evaluationCount = problem.Evaluate(Population);
			Population.Sort();
			//Console.WriteLine("Best:\n{0}", Population.GetFittest());
			Logger.WriteLine(1, string.Format("Evaluated " + evaluationCount + "/" + Population.Size + " individuals"));
			Logger.WriteLine(1, string.Format("Best fitness: {0}, Avg: {1}", Population.GetFittest().Fitness, Population.GetAverageFitness()));

			var isMDL = false;
			if (Population.GetFittest().FitnessResult != null && Population.GetFittest().FitnessResult is MDLFitnessResult) {
				isMDL = true;
			}

			// Write statistics in .csv format
			Logger.WriteLine(0, string.Format("{0};{1};{2};{3};{4};{5};{6};{7}",
				generation,
				Population.GetFittest().Fitness,
				isMDL ? ((MDLFitnessResult) Population.GetFittest().FitnessResult).StandardFitness : 0,
				isMDL ? ((MDLFitnessResult) Population.GetFittest().FitnessResult).ClassificationError : 0,
				isMDL ? ((MDLFitnessResult) Population.GetFittest().FitnessResult).TreeSize : 0,
				Population.GetAverageFitness(),
				evaluationCount,
				ComputeDistanceToOptimal(Population.GetFittest(), problem)));

			if (IsSolutionFound()) {
				Logger.WriteLine(1, "Solution found.");
				Logger.WriteLine(2, string.Format("Solution:\n{0}", Population.GetFittest()));
				return Population.GetFittest();
			}
			return null;
		}

		private int ComputeDistanceToOptimal(Individual individual, Problem problem) {
			int minDistance = int.MaxValue;
			foreach (var solution in problem.GetOptimalSolutions()) {
				int distance = SimilarityMeasure.Measure(individual.SyntaxTree, solution);
				if (distance < minDistance) {
					minDistance = distance;
				}
			}
			return minDistance;
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
				double rand = RandomValueGenerator.Instance.GetDouble();
				if (rand < CrossoverRate) {
					var individual1 = Selector.Select(population);
					var individual2 = Selector.Select(population);
					foreach (var child in Crossover.cross(individual1, individual2)) {
						children.Add(new Individual(child));
					}
				}
				else if (rand - CrossoverRate < MutationRate) {
					var child = new Individual(Selector.Select(population));
					if (Mutator.Mutate(child)) {
						child.FitnessEvaluated = false;
					}
					children.Add(child);
				}
				else {
					children.Add(new Individual(Selector.Select(population)));
				}
				foreach (var child in children) {
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
