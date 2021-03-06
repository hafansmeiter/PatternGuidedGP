﻿using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.SimilarityEvaluation.TreeEditDistance;
using PatternGuidedGP.GP.Problems;
using PatternGuidedGP.Pangea;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP {
	class GPAlgorithm : AlgorithmBase {

		// track fitness for termination criteria:
		// if best fitness and average fitness not improved for 20 generations -> abort
		private int _stagnationGen = 20;
		private double[] _bestFitness;
		private double[] _avgFitness;

		public GPAlgorithm(int populationSize, int generations, bool allowDuplicates)
			: base(populationSize, generations, allowDuplicates) {
			_bestFitness = new double[generations];
			_avgFitness = new double[generations];
		}

		public override Individual Run(Problem problem) {
			Initializer.Initialize(Population, problem.RootType);
			Logger.WriteLine(1, "Generation 0: ");
			// .csv header
			Logger.WriteLine(0, "Generation;Best_fitness;Best_program_error;Best_classification_error;Best_tree_size;Avg_fitness;Evaluated;Total_evaluated;Dist_to_optimal;Diversity_best_to_avg;Diversity_tree_dist");
			Logger.WriteStatisticsHeader();
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

				_bestFitness[i] = Population.GetFittest().Fitness;
				_avgFitness[i] = Population.GetAverageFitness();
				if (IsStagnating(i)) {
					break;
				}
			}
			Logger.WriteLine(1, "No solution found.");
			Logger.WriteLine(2, string.Format("Returning best:\n{0}", Population.GetFittest()));
			return Population.GetFittest();
		}

		private bool IsStagnating(int generation) {
			if (generation < _stagnationGen) {
				return false;
			}
			return _bestFitness[generation] >= _bestFitness[generation - _stagnationGen]
				&& _avgFitness[generation] >= _avgFitness[generation - _stagnationGen];
		}

		private Individual EvaluatePopulation(Problem problem, int generation) {
			int evaluationCount = problem.Evaluate(Population);
			TotalEvaluations += evaluationCount;
			Population.Sort();
			Logger.WriteLine(1, string.Format("Best:\n{0}", Population.GetFittest()));

			var isMDL = false;
			if (Population.GetFittest().FitnessResult != null && Population.GetFittest().FitnessResult is MDLFitnessResult) {
				isMDL = true;
			}

			// Write statistics in .csv format
			CollectNodeTypes();
			Logger.WriteStatistics(generation, new string[] { "total", "for", "if", "&&", "||", "%", "1", "2", "==", "!=", "ret", "i", "values", "length", "n", "a", "b", "c", "d", "<", "<=", ">", ">=", "&&==abcd", "==abcd", "||!=abcd", "!=abcd" });

			double averageFitness = Population.GetAverageFitness();
			Logger.WriteLine(0, string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}",
				generation,
				Population.GetFittest().Fitness,
				isMDL ? ((MDLFitnessResult) Population.GetFittest().FitnessResult).StandardFitness : 0,
				isMDL ? ((MDLFitnessResult) Population.GetFittest().FitnessResult).ClassificationError : 0,
				isMDL ? ((MDLFitnessResult) Population.GetFittest().FitnessResult).TreeSize : 0,
				averageFitness,
				evaluationCount,
				TotalEvaluations,
				ComputeDistanceToOptimal(Population.GetFittest(), problem),
				averageFitness - Population.GetFittest().Fitness,
				Population.GetDiversity(SimilarityMeasure)));

			Statistics.Instance.ClearAll();

			if (IsSolutionFound()) {
				Logger.WriteLine(1, "Solution found.");
				Logger.WriteLine(2, string.Format("Solution:\n{0}", Population.GetFittest()));
				return Population.GetFittest();
			}
			return null;
		}

		private void CollectNodeTypes() {
			foreach (var individual in Population.Individuals) {
				foreach (var node in individual.SyntaxTree.GetTreeNodes()) {
					Statistics.Instance.AddNodeType(node.Description);

					if (node.Description == "&&" 
						&& IsEqualsParameterVariable(node.Children[0])
						&& IsEqualsParameterVariable(node.Children[1])) {
						Statistics.Instance.AddNodeType("&&==abcd");
					} else if (IsEqualsParameterVariable(node)) {
						Statistics.Instance.AddNodeType("==abcd");
					}

					if (node.Description == "||"
						&& IsNotEqualsParameterVariable(node.Children[0])
						&& IsNotEqualsParameterVariable(node.Children[1])) {
						Statistics.Instance.AddNodeType("||!=abcd");
					}
					else if (IsNotEqualsParameterVariable(node)) {
						Statistics.Instance.AddNodeType("!=abcd");
					}
				}
			}
		}

		private bool IsEqualsParameterVariable(TreeNode node) {
			return node.Description == "=="
				&& IsParameterVariable(node.Children[0])
				&& IsParameterVariable(node.Children[1]);
		}

		private bool IsNotEqualsParameterVariable(TreeNode node) {
			return node.Description == "!="
				&& IsParameterVariable(node.Children[0])
				&& IsParameterVariable(node.Children[1]);
		}

		private bool IsParameterVariable(TreeNode node) {
			return node.Description == "a" ||
				node.Description == "b" ||
				node.Description == "c" ||
				node.Description == "d";
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
					foreach (var child in Crossover.Cross(individual1, individual2)) {
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
