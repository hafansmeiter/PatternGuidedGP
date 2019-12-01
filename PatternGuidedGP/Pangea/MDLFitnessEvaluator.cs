﻿using Accord.MachineLearning.DecisionTrees;
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

		protected override void PrepareTestRuns(Individual individual, TestSuite testSuite) {
			Singleton<ExecutionTraces>.Instance.Reset();
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
	}
}
