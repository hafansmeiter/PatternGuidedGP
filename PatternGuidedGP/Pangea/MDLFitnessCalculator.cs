using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math.Optimization.Losses;
using PatternGuidedGP.GP;
using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Pangea {
	class MDLFitnessCalculator : IFitnessCalculator {
		
		public IFitnessCalculator StandardFitnessCalculator { get; set; } = new EqualityFitnessCalculator();
		public static int Steps = 5;

		public MDLFitnessCalculator() {
		}

		public FitnessResult CalculateFitness(Individual individual, TestSuite testSuite, object[] results) {
			double fitness = StandardFitnessCalculator.CalculateFitness(individual, testSuite, results).Fitness; // standard fitness f0
			MLDataset dataset = MLDataset.FromExecutionTraces(individual, Singleton<ExecutionRecord>.Instance.Traces);
			LogDatasetFeatures(dataset);

			var fitnessResult = new MDLFitnessResult(fitness, dataset);
			if (dataset.Features.Count() > 0) {
				// Variant 1 (results of all nodes):
				//var input = dataset.ToRawInputDataset();

				// Variant 2 (results and operation types of first n and last n operations in chronological order):
				//var input = dataset.ToRawFirstNLastNInputDataset(5, 5);

				// Veriant 3 (results only consider values of bool and int expressions and operations)
				int startContinuousFeatures;
				var input = MLDataset.ConvertTracesToTypedSteps(individual, Singleton<ExecutionRecord>.Instance.Traces, 
					Steps, true, out startContinuousFeatures);

				if (input.Length > 0 && input[0].Length > 0) {
					var expected = GetExpectedOutputDataset(testSuite);
					LogDataset(input, expected);

					var decisionTree = CreateDecisionTree(input, expected, startContinuousFeatures);
					if (decisionTree != null) {
						int error = GetClassificationError(decisionTree, input, expected);
						int treeSize = GetTreeSize(decisionTree);
						double mdlFitness = CalculateMDLFitness(error, treeSize, results.Length);
						double stdFitness = fitness;
						fitness *= mdlFitness;

						var rules = decisionTree.ToRules();
						LogResult(fitness, error, treeSize, mdlFitness, rules);

						fitnessResult.Fitness = fitness;
						fitnessResult.ClassificationError = error;
						fitnessResult.TreeSize = treeSize;
						fitnessResult.StandardFitness = stdFitness;

						// log MDL result details
						if (fitness == 0) {
							Logger.WriteLine(1, "Std fitness: " + stdFitness);

							var predicted = decisionTree.Decide(input);
							var loss = Math.Round(new ZeroOneLoss(expected).Loss(predicted) * expected.Length);
							var classificationErrorFactor = (((double)error + 1) / (results.Length + 1));
							Logger.WriteLine(1, "Program error: " + error + "; loss: " + loss + "; factor: " + classificationErrorFactor);

							var treeSizeFactor = Math.Log(treeSize + 1, 2);
							Logger.WriteLine(1, "Tree size: " + treeSize + "; factor: " + treeSizeFactor);

							Logger.WriteLine(1, "MDL fitness: " + mdlFitness);
						}
					}
				}
			} else if (fitness == 0) {
				Logger.WriteLine(1, "Standard fitness: 0");
			}
			return fitnessResult;
		}

		private double CalculateMDLFitness(int error, int treeSize, int n) {
			var treeSizeFactor = Math.Log(treeSize + 1, 2);
			var classificationErrorFactor = (((double)error + 1) / (n + 1));
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

		private int GetClassificationError(DecisionTree decisionTree, int?[][] input, int[] expected) {
			var predicted = decisionTree.Decide(input);
			return (int) (Math.Round(new ZeroOneLoss(expected).Loss(predicted) * expected.Length));
		}

		private DecisionTree CreateDecisionTree(int?[][] input, int[] output, int startContinuousVariables) {
			var variables = DecisionVariable.FromData(input);
			if (startContinuousVariables >= 0) {
				for (int i = startContinuousVariables; i < variables.Length; i++) {
					variables[i] = DecisionVariable.Continuous(i.ToString());
				}
			}
			var learner = new C45Learning(variables);
			DecisionTree tree = null;
			try {
				tree = learner.Learn(input, output);
			}
			catch (Exception e) {
				//Console.WriteLine(e.Message);
				//Console.WriteLine(e.ToString());
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
			if (dataset != null) {
				Logger.Write(4, "Features: ");
				foreach (var feature in dataset.Features) {
					Logger.Write(4, feature.ToString() + ",");
				}
				Logger.WriteLine(4, "");
			}
		}
	}
}
