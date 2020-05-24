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
	class MDLFitnessEvaluator : ProgramFitnessEvaluator {
		
		public ISubTreePool SubTreePool { get; set; }

		public MDLFitnessEvaluator() {
			FitnessCalculator = new MDLFitnessCalculator();	// proxy
		}

		protected override void PrepareTestRuns(Individual individual, TestSuite testSuite) {
			Singleton<ExecutionRecord>.Instance.Reset();
		}

		protected override void OnTestRunFinished(Individual individual, TestCase testCase, object result) {
			Singleton<ExecutionRecord>.Instance.FinishCurrent();
		}

		protected override void OnIndividualEvaluationFinished(Individual individual, FitnessResult fitness, object[] results) {
			MDLFitnessResult fitnessResult = fitness as MDLFitnessResult;
			if (SubTreePool != null) {
				int usedAttributes = fitnessResult.UsedAttributes;
				int classificationError = fitnessResult.ClassificationError;
				if (usedAttributes > 0) {
					double utilityMeasure = CalculateUtilityMeasure(usedAttributes, classificationError);
					foreach (var id in fitnessResult.Dataset.Features) {
						SubTreePool.Add(individual.SyntaxTree.FindNodeById(id), utilityMeasure);
					}
					SubTreePool.TrimToMaxSize();
				}
			}
		}

		public override void OnEvaluationFinished() {
		}

		private double CalculateUtilityMeasure(int usedAttributes, int classificationError) {
			return 1.0 / ((1.0 + classificationError) * usedAttributes);
		}

		public override void OnStartEvaluation() {
			if (SubTreePool != null) {
				SubTreePool.RemoveWorstItems();
			}
		}

		protected override CompilationUnitSyntax CreateCompilationUnit(Individual individual, TestCase sample, CompilationUnitSyntax template) {
			//var syntax = individual.SyntaxTree.GetSyntaxNode();
			var compSyntax = base.CreateCompilationUnit(individual, sample, template);

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
			// ret = a + b;
			// Store execution trace for code (23, 1, 2, 3 are node ids):
			// e.g.: ExecutionRecord.Current.Add(result node id: 23, operation node id: 1, result: a + b, operation_code: 201);
			// try {
			//   ExecutionRecord.Current.Add(23, 1, a + b, 201);
			// } catch (Exception e) {}
			// try {
			//   ExecutionRecord.Current.Add(23, 2, a, -1);
			// } catch (Exception e) {}
			// try {
			//   ExecutionRecord.Current.Add(23, 3, b, -1);
			// } catch (Exception e) {}

			var statements = new List<StatementSyntax>();
			statements.AddRange(((ITraceable) tracedNode).GetExecutionTraceNodes().Select(node =>
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
														SyntaxFactory.IdentifierName("ExecutionRecord")))),
											SyntaxFactory.IdentifierName("Instance")),
										SyntaxFactory.IdentifierName("Current")),
									SyntaxFactory.IdentifierName("Add")))
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
												(ExpressionSyntax) node.GetExecutionStateSyntaxNode()),
											SyntaxFactory.Argument(
												SyntaxFactory.LiteralExpression(
													SyntaxKind.NumericLiteralExpression,
													SyntaxFactory.Literal(node.OperatorId)))
										}
									)
								)
							)
						)
					)
				)));
			return SyntaxFactory.Block(statements);
		}
	}
}
