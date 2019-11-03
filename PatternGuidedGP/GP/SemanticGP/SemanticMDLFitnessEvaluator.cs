using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.Pangea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	class SemanticMDLFitnessEvaluator : MDLFitnessEvaluator {

		protected override void OnEvaluationFinished(Individual individual, FitnessResult fitness) {
			base.OnEvaluationFinished(individual, fitness);

			var fitnessResult = fitness as MDLFitnessResult;
			var dataset = fitnessResult.Dataset;
			foreach (var id in dataset.Features) {
				var node = individual.SyntaxTree.FindNodeById(id);
				var semanticsNode = node as ISemanticsProvider;
				if (semanticsNode != null) {
					int testCount = dataset.Count;
					semanticsNode.IsSemanticsEvaluated = true;
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
