﻿using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Pangea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	class SemanticMDLFitnessEvaluator : MDLFitnessEvaluator {

		protected override void PrepareTestRuns(Individual individual, TestSuite testSuite) {
			base.PrepareTestRuns(individual, testSuite);
			individual.SemanticsEvaluated = false;
			foreach (var node in individual.SyntaxTree.GetTreeNodes()) {
				var semanticNode = node as ISemanticsProvider;
				if (semanticNode != null) {
					semanticNode.SemanticsEvaluated = false;
				}
			}
		}

		protected override void OnEvaluationFinished(Individual individual, FitnessResult fitness, object[] results) {
			base.OnEvaluationFinished(individual, fitness, results);

			AssignSemanticsToTreeNodes(individual, fitness as MDLFitnessResult);
			AssignSemanticsToIndividual(individual, results);
		}

		private void AssignSemanticsToIndividual(Individual individual, object[] results) {
			individual.Semantics = new Semantics(results);
		}

		private void AssignSemanticsToTreeNodes(Individual individual, MDLFitnessResult fitnessResult) {
			var dataset = fitnessResult.Dataset;
			int testCount = dataset.Count;
			foreach (var id in dataset.Features) {
				var node = individual.SyntaxTree.FindNodeById(id);
				var semanticsNode = node as ISemanticsProvider;
				if (semanticsNode != null) {
					var semantics = new Semantics(testCount);
					for (int i = 0; i < testCount; i++) {
						semantics[i] = dataset[id][i];
					}
					semanticsNode.Semantics = semantics;
				}
			}
		}
	}
}
