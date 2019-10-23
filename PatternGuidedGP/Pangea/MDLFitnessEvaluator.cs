using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math.Optimization.Losses;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.Pool;
using PatternGuidedGP.AbstractSyntaxTree.SyntaxGenerator;
using PatternGuidedGP.GP;
using PatternGuidedGP.GP.Problems;
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Pangea {

	// MDL = Minimum description length: https://en.wikipedia.org/wiki/Minimum_description_length
	class MDLFitnessEvaluator : DefaultFitnessEvaluator {
		class MDLFitnessResult : FitnessResult {
			public IEnumerable<ulong> Features { get; private set; }

			public MDLFitnessResult(double fitness, IEnumerable<ulong> features)
				: base(fitness) {
				Features = features;
			}
		}

		public ISubTreePool SubTreePool { get; set; }

		protected override void PrepareTestRuns(Individual individual, TestSuite testSuite) {
			ExecutionTraces.Reset();
		}

		protected override FitnessResult CalculateFitness(Individual individual, TestSuite testSuite, object[] results) {
			double fitness = base.CalculateFitness(individual, testSuite, results).Fitness;	// standard fitness f0

			var dataset = MLDataset.FromExecutionTraces(individual, ExecutionTraces.Traces);

			Logger.Write(4, "Features: ");
			foreach (var feature in dataset.Features) {
				Logger.Write(4, feature.ToString() + ",");
			}
			Logger.WriteLine(4, "");

			if (dataset.Features.Count() > 0) {
				var input = dataset.ToRawInputDataset();

				var expected = GetExpectedOutputDataset(testSuite);
				var actual = GetActualOutputDataset(results);

				// Log input matrix and outputs
				Logger.WriteLine(4, "\nInput:");
				for (int i = 0; i < input.Length; i++) {
					Logger.Write(4, "\n" + i + ": ");
					for (int j = 0; j < input[i].Length; j++) {
						Logger.Write(4, input[i][j] + ", ");
					}
					Logger.Write(4, "Exp: " + expected[i]);
					Logger.Write(4, ", Act: " + actual[i]);
				}
				Logger.WriteLine(4, "");

				var decisionTree = CreateDecisionTree(input, expected);
				if (decisionTree != null) {
					double error = GetClassificationError(decisionTree, expected, actual);
					int treeLength = GetTreeLength(decisionTree);

					double mdlFitness = CalculateMDLFitness(error, treeLength, results.Length);

					var rules = decisionTree.ToRules();

					Logger.WriteLine(3, "Error: " + error + ", Tree length: " + treeLength);
					Logger.WriteLine(3, "Fitness: " + fitness * mdlFitness + ", default fitness: " + fitness + ", mdl fitness: " + mdlFitness);
					Logger.WriteLine(3, "Rules: " + rules.ToString());
					if (mdlFitness == 0) {
						Logger.WriteLine(3, "MDL fitness is zero.");
					}
					fitness *= mdlFitness;
				}
			}
			return new MDLFitnessResult(fitness, dataset.Features);
		}

		private double CalculateMDLFitness(double error, int treeLength, int n) {
			return Math.Log(treeLength + 1, 2) * ((error + 1) / (n + 1));
		}

		private int GetTreeLength(DecisionTree decisionTree) {
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

		private double GetClassificationError(DecisionTree decisionTree, int[] expected, int[] actual) {
			return Math.Round(new ZeroOneLoss(expected).Loss(actual) * expected.Length);
		}

		private DecisionTree CreateDecisionTree(int[][] input, int[] output) {
			var learner = new C45Learning();
			DecisionTree tree = null;
			try {
				tree = learner.Learn(input, output);
			} catch (Exception e) { }
			return tree;
		}

		private int[] GetActualOutputDataset(object[] results) {
			int[] dataset = new int[results.Length];
			for (int i = 0; i < dataset.Length; i++) {
				dataset[i] = MLDataset.ToDatasetValue(results[i]);
			}
			return dataset;
		}

		private int[] GetExpectedOutputDataset(TestSuite testSuite) {
			int[] dataset = new int[testSuite.TestCases.Count];
			for (int i = 0; i < dataset.Length; i++) {
				dataset[i] = MLDataset.ToDatasetValue(testSuite.TestCases[i].Result);
			}
			return dataset;
		}

		protected override void OnTestRunFinished(Individual individual, TestCase testCase, object result) {
			ExecutionTraces.FinishCurrent();
		}

		protected override void OnEvaluationFinished(Individual individual, FitnessResult fitness) {
			MDLFitnessResult fitnessResult = fitness as MDLFitnessResult;
			if (SubTreePool != null) {
				double fitnessValue = fitnessResult.Fitness;
				foreach (var id in fitnessResult.Features) {
					SubTreePool.Add(individual.SyntaxTree.FindNodeById(id), fitnessValue);
				}
			}
		}

		protected override CompilationUnitSyntax CreateCompilationUnit(Individual individual, TestCase sample, CompilationUnitSyntax template) {
			var syntax = individual.SyntaxTree.Root.GetSyntaxNode();
			var compSyntax = base.CreateCompilationUnit(syntax, sample, template);

			var traceableNodes = individual.SyntaxTree.GetTraceableNodes();
			foreach (var traceableNode in traceableNodes) {
				var assignmentSyntaxNode = compSyntax.FindNodeById(traceableNode.Id);		
				var storageCallSyntax = GetStorageCallSyntax(traceableNode);
				compSyntax = compSyntax.InsertStatementBefore(assignmentSyntaxNode, storageCallSyntax);
			}
			return compSyntax;
		}

		private SyntaxNode GetStorageCallSyntax(TreeNode tracedNode) {
			// Code example: 
			// ret = a <= b;
			// Store execution trace for code (23, 1, 2, 3 are node ids):
			// ExecutionTraces.Current.Add(23, new ExecutionRecord(1, a <= b), new ExecutionRecord(2, a), new ExecutionRecord(3, b));

			var executionTraceParameters = new List<ArgumentSyntax> {
								SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(
									SyntaxKind.NumericLiteralExpression,
									SyntaxFactory.Literal(tracedNode.Id)))
			};
			executionTraceParameters.AddRange(tracedNode.GetExecutionTraceNodes().Select(node => SyntaxFactory.Argument(
									SyntaxFactory.ObjectCreationExpression(
										SyntaxFactory.IdentifierName("ExecutionRecord"))
									.WithArgumentList(
										SyntaxFactory.ArgumentList(
											SyntaxFactory.SeparatedList(
												new ArgumentSyntax[] {
													SyntaxFactory.Argument(
														SyntaxFactory.LiteralExpression(
															SyntaxKind.NumericLiteralExpression,
															SyntaxFactory.Literal(node.Id))),
													SyntaxFactory.Argument(
														(ExpressionSyntax) node.GetSyntaxNode())
												}
											)
										)
									)
								)));
			return SyntaxFactory.ExpressionStatement(
				SyntaxFactory.InvocationExpression(
					SyntaxFactory.MemberAccessExpression(
						SyntaxKind.SimpleMemberAccessExpression,
						SyntaxFactory.MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							SyntaxFactory.IdentifierName("ExecutionTraces"),
							SyntaxFactory.IdentifierName("Current")),
						SyntaxFactory.IdentifierName("Add")))
				.WithArgumentList(
					SyntaxFactory.ArgumentList(
						SyntaxFactory.SeparatedList<ArgumentSyntax>(
							executionTraceParameters
						)
					)
				)
			);
		}
	}
}
