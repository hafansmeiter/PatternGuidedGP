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
using PatternGuidedGP.GP.Evaluators;
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
		protected class MDLFitnessResult : FitnessResult {
			public MLDataset Dataset { get; private set; }

			public MDLFitnessResult(double fitness, MLDataset dataset)
				: base(fitness) {
				Dataset = dataset;
			}
		}

		public ISubTreePool SubTreePool { get; set; }

		protected override void PrepareTestRuns(Individual individual, TestSuite testSuite) {
			Singleton<ExecutionTraces>.Instance.Reset();
		}

		protected override FitnessResult CalculateFitness(Individual individual, TestSuite testSuite, object[] results) {
			double fitness = base.CalculateFitness(individual, testSuite, results).Fitness; // standard fitness f0
			var dataset = MLDataset.FromExecutionTraces(individual, Singleton<ExecutionTraces>.Instance.Traces);
			LogDatasetFeatures(dataset);

			if (dataset.Features.Count() > 0) {
				var input = dataset.ToRawInputDataset();

				var expected = GetExpectedOutputDataset(testSuite);
				var actual = GetActualOutputDataset(results, testSuite.TestCases[0].Result.GetType());

				LogDataset(input, expected, actual);

				var decisionTree = CreateDecisionTree(input, expected);
				if (decisionTree != null) {
					double error = GetClassificationError(decisionTree, expected, actual);
					int treeLength = GetTreeLength(decisionTree);
					double mdlFitness = CalculateMDLFitness(error, treeLength, results.Length);
					fitness *= mdlFitness;

					var rules = decisionTree.ToRules();
					LogResult(fitness, error, treeLength, mdlFitness, rules);
				}
			}
			return new MDLFitnessResult(fitness, dataset);
		}

		protected override void OnTestRunFinished(Individual individual, TestCase testCase, object result) {
			Singleton<ExecutionTraces>.Instance.FinishCurrent();
		}

		protected override void OnEvaluationFinished(Individual individual, FitnessResult fitness, object[] results) {
			MDLFitnessResult fitnessResult = fitness as MDLFitnessResult;
			if (SubTreePool != null) {
				double fitnessValue = fitnessResult.Fitness;
				foreach (var id in fitnessResult.Dataset.Features) {
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

		private int[] GetActualOutputDataset(object[] results, Type type) {
			int[] dataset = new int[results.Length];
			for (int i = 0; i < dataset.Length; i++) {
				if (type == typeof (bool)) {
					// ensure no result will be counted as false result
					dataset[i] = MLDataset.ToDatasetValue(results[i]) ?? -1;
				} else {
					dataset[i] = MLDataset.ToDatasetValue(results[i]).GetValueOrDefault();
				}
			}
			return dataset;
		}

		private int[] GetExpectedOutputDataset(TestSuite testSuite) {
			int[] dataset = new int[testSuite.TestCases.Count];
			for (int i = 0; i < dataset.Length; i++) {
				dataset[i] = MLDataset.ToDatasetValue(testSuite.TestCases[i].Result).GetValueOrDefault();
			}
			return dataset;
		}

		private SyntaxNode GetStorageCallSyntax(TreeNode tracedNode) {
			// Code example: 
			// ret = a + b;
			// Store execution trace for code (23, 1, 2, 3 are node ids):
			// try {
			//   ExecutionTraces.Current.Add(23, new ExecutionRecord(1, a + b));
			// } catch (Exception e) {}
			// try {
			//   ExecutionTraces.Current.Add(23, new ExecutionRecord(2, a));
			// } catch (Exception e) {}
			// try {
			//   ExecutionTraces.Current.Add(23, new ExecutionRecord(3, b));
			// } catch (Exception e) {}

			var statements = new List<StatementSyntax>();
			statements.AddRange(tracedNode.GetExecutionTraceNodes().Select(node =>
				SyntaxFactory.TryStatement(
					SyntaxFactory.SingletonList<CatchClauseSyntax>(
						SyntaxFactory.CatchClause()
						.WithDeclaration(
							SyntaxFactory.CatchDeclaration(
								SyntaxFactory.IdentifierName("Exception"))
							.WithIdentifier(
								SyntaxFactory.Identifier("ex")))))
				.WithBlock(
					SyntaxFactory.Block(
						SyntaxFactory.ExpressionStatement(
							SyntaxFactory.InvocationExpression(
								SyntaxFactory.MemberAccessExpression(
									SyntaxKind.SimpleMemberAccessExpression,
									SyntaxFactory.MemberAccessExpression(
										SyntaxKind.SimpleMemberAccessExpression,
										SyntaxFactory.MemberAccessExpression(
											SyntaxKind.SimpleMemberAccessExpression,
											SyntaxFactory.GenericName(
												SyntaxFactory.Identifier("Singleton"))
											.WithTypeArgumentList(
												SyntaxFactory.TypeArgumentList(
													SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
														SyntaxFactory.IdentifierName("ExecutionTraces")))),
											SyntaxFactory.IdentifierName("Instance")),
										SyntaxFactory.IdentifierName("Current")),
									SyntaxFactory.IdentifierName("Add")))
							// does not work cross-appdomain 
							/*SyntaxFactory.InvocationExpression(
								SyntaxFactory.MemberAccessExpression(
									SyntaxKind.SimpleMemberAccessExpression,
									SyntaxFactory.MemberAccessExpression(
										SyntaxKind.SimpleMemberAccessExpression,
										SyntaxFactory.IdentifierName("ExecutionTraces"),
										SyntaxFactory.IdentifierName("Current")),
									SyntaxFactory.IdentifierName("Add")))*/
							.WithArgumentList(
								SyntaxFactory.ArgumentList(
									SyntaxFactory.SeparatedList<ArgumentSyntax>(
										new ArgumentSyntax[] {
											SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(
												SyntaxKind.NumericLiteralExpression,
												SyntaxFactory.Literal(tracedNode.Id))),
											SyntaxFactory.Argument(
												SyntaxFactory.LiteralExpression(
													SyntaxKind.NumericLiteralExpression,
													SyntaxFactory.Literal(node.Id))),
											SyntaxFactory.Argument(
												(ExpressionSyntax) node.GetSyntaxNode())
											/*SyntaxFactory.Argument(
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
												)*/
											}
										)
									)
								)
							)
						)
					)));
			return SyntaxFactory.Block(statements);
		}

		private void LogResult(double fitness, double error, int treeLength, double mdlFitness, Accord.MachineLearning.DecisionTrees.Rules.DecisionSet rules) {
			Logger.WriteLine(3, "Error: " + error + ", Tree length: " + treeLength);
			Logger.WriteLine(3, "Fitness: " + fitness * mdlFitness + ", default fitness: " + fitness + ", mdl fitness: " + mdlFitness);
			Logger.WriteLine(3, "Rules: " + rules.ToString());
			if (mdlFitness == 0) {
				Logger.WriteLine(3, "MDL fitness is zero.");
			}
		}

		private void LogDataset(int?[][] input, int[] expected, int[] actual) {
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
