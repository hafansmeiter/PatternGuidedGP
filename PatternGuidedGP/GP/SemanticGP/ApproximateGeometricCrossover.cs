using PatternGuidedGP.GP;
using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.GP.Operators;
using PatternGuidedGP.GP.Problems;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {

	// Pawlak et al. - Semantic Backpropagation for Designing Search Operators in GP: page 332
	// AGX Crossover
	class ApproximateGeometricCrossover : ICrossover, IGeometricOperator {

		public IGeometricCalculator GeometricCalculator { get; set; }
		public ISemanticSubTreePool SubTreePool { get; set; }
		public IResultSemanticsOperator ResultSemanticsOperator { get; set; } = new PawlakRandomDesiredOperator();
		public int MaxTreeDepth { get; set; }
		public ICrossover Fallback { get; set; }

		public IFitnessEvaluator FitnessEvaluator { get; set; }
		public Problem Problem { get; set; }

		public ApproximateGeometricCrossover(ISemanticSubTreePool subTreePool, int maxTreeDepth) {
			SubTreePool = subTreePool;
			MaxTreeDepth = maxTreeDepth;
		}

		public IEnumerable<Individual> Cross(Individual individual1, Individual individual2) {
			var midpoint = GeometricCalculator.GetMidpoint(individual1.Semantics, individual2.Semantics);

			bool triedBackPropagation1;
			var child1 = GenerateChildren(individual1, individual2, midpoint, out triedBackPropagation1);

			bool triedBackPropagation2;
			var child2 = GenerateChildren(individual2, individual1, midpoint, out triedBackPropagation2);

			if (!triedBackPropagation1 && !triedBackPropagation2 && Fallback != null) {
				return Fallback.Cross(individual1, individual2);
			}

			return new[] { child1, child2 };
		}

		private Individual GenerateChildren(Individual individual1, Individual individual2, 
			Semantics midpoint, out bool triedBackPropagation) {
			var child1 = new Individual(individual1);
			child1.FitnessEvaluated = false;

			var mutated = ResultSemanticsOperator.Operate(midpoint,
				child1, SubTreePool, MaxTreeDepth, out triedBackPropagation);

			double fitnessChange = 0;
			if (mutated && triedBackPropagation && individual1.FitnessEvaluated && FitnessEvaluator != null) {
				double fitness = FitnessEvaluator.Evaluate(child1, Problem).Fitness;
				fitnessChange = fitness - individual1.Fitness;
			}
			Statistics.Instance.AddBackpropagationAttemptCrossover(triedBackPropagation, fitnessChange);

			return mutated ? child1 : individual1;
		}
	}
}
