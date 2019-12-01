using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math.Optimization.Losses;
using PatternGuidedGP.GP;
using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Pangea {
	class MDLFitnessCalculator : IFitnessCalculator {
		
		private IFitnessCalculator _standardFitnessCalculator = new EqualityFitnessCalculator();

		public FitnessResult CalculateFitness(Individual individual, TestSuite testSuite, object[] results) {
			double fitness = _standardFitnessCalculator.CalculateFitness(individual, testSuite, results).Fitness; // standard fitness f0
			var dataset = MLDataset.FromExecutionTraces(individual, Singleton<ExecutionTraces>.Instance.Traces);
			LogDatasetFeatures(dataset);

			if (dataset.Features.Count() > 0) {
				var input = dataset.ToRawInputDataset();
				var expected = GetExpectedOutputDataset(testSuite);
				LogDataset(input, expected);

				var decisionTree = CreateDecisionTree(input, expected);
				if (decisionTree != null) {
					double error = GetClassificationError(decisionTree, input, expected);
					int treeSize = GetTreeSize(decisionTree);
					double mdlFitness = CalculateMDLFitness(error, treeSize, results.Length);
					fitness *= mdlFitness;

					var rules = decisionTree.ToRules();
					LogResult(fitness, error, treeSize, mdlFitness, rules);
				}
			}
			return new MDLFitnessResult(fitness, dataset);
		}

		private double CalculateMDLFitness(double error, int treeSize, int n) {
			var treeSizeFactor = Math.Log(treeSize + 1, 2);
			var classificationErrorFactor = ((error + 1) / (n + 1));
			return treeSizeFactor * classificationErrorFactor;
		}

		private int GetTreeSize(DecisionTree decisionTree) {
			var root = decisionTree.Root;
			int length = 0;
			GetSubtreeLength(root, ref length);
			return length;
		}

		private void GetSubtreeLength(DecisionNode node, ref int length) {
			length++;
			if (node.IsLeaf) {
				return;
			}
			foreach (var branch in node.Branches) {
				GetSubtreeLength(branch, ref length);
			}
		}

		private double GetClassificationError(DecisionTree decisionTree, int?[][] input, int[] expected) {
			var actual = decisionTree.Decide(input);
			return Math.Round(new ZeroOneLoss(expected).Loss(actual) * expected.Length);
		}

		private DecisionTree CreateDecisionTree(int?[][] input, int[] output) {
			var learner = new C45Learning();
			DecisionTree tree = null;
			try {
				tree = learner.Learn(input, output);
			}
			catch (Exception e) {
				Console.WriteLine(e.Message);
				Console.WriteLine(e.ToString());
			}
			return tree;
		}

		private int[] GetExpectedOutputDataset(TestSuite testSuite) {
			int[] dataset = new int[testSuite.TestCases.Count];
			for (int i = 0; i < dataset.Length; i++) {
				dataset[i] = MLDataset.ToDatasetValue(testSuite.TestCases[i].Result).GetValueOrDefault();
			}
			return dataset;
		}

		private void LogResult(double fitness, double error, int treeLength, double mdlFitness, Accord.MachineLearning.DecisionTrees.Rules.DecisionSet rules) {
			Logger.WriteLine(3, "Error: " + error + ", Tree length: " + treeLength);
			Logger.WriteLine(3, "Fitness: " + fitness * mdlFitness + ", default fitness: " + fitness + ", mdl fitness: " + mdlFitness);
			Logger.WriteLine(3, "Rules: " + rules.ToString());
			if (mdlFitness == 0) {
				Logger.WriteLine(3, "MDL fitness is zero.");
			}
		}

		private void LogDataset(int?[][] input, int[] expected) {
			// Log input matrix and outputs
			Logger.WriteLine(4, "\nInput:");
			for (int i = 0; i < input.Length; i++) {
				Logger.Write(4, "\n" + i + ": ");
				for (int j = 0; j < input[i].Length; j++) {
					Logger.Write(4, input[i][j] + ", ");
				}
				Logger.Write(4, " | " + expected[i]);
			}
			Logger.WriteLine(4, "");
		}

		private void LogDatasetFeatures(MLDataset dataset) {
			Logger.Write(4, "Features: ");
			foreach (var feature in dataset.Features) {
				Logger.Write(4, feature.ToString() + ",");
			}
			Logger.WriteLine(4, "");
		}
	}
}
