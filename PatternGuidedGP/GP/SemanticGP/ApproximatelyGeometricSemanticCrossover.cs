using PatternGuidedGP.GP;
using PatternGuidedGP.GP.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {

	// Pawlak et al. - Semantic Backpropagation for Designing Search Operators in GP: page 332
	// AGX Crossover
	class ApproximatelyGeometricSemanticCrossover : ICrossover, IGeometricOperator {

		public IGeometricCalculator GeometricCalculator { get; set; }
		public ISemanticSubTreePool SubTreePool { get; set; }
		public IResultSemanticsOperator ResultSemanticsOperator { get; set; } = new PawlakRandomDesiredOperator();
		public int MaxTreeDepth { get; set; }

		public ApproximatelyGeometricSemanticCrossover(ISemanticSubTreePool subTreePool, int maxTreeDepth) {
			SubTreePool = subTreePool;
			MaxTreeDepth = maxTreeDepth;
		}

		public IEnumerable<Individual> cross(Individual individual1, Individual individual2) {
			var midpoint = GeometricCalculator.GetMidpoint(individual1.Semantics, individual2.Semantics);

			var child1 = new Individual(individual1);
			child1.FitnessEvaluated = false;
			var child2 = new Individual(individual2);
			child2.FitnessEvaluated = false;
			bool mutatedChild1 = ResultSemanticsOperator.Operate(midpoint,
				child1, SubTreePool, MaxTreeDepth);
			bool mutatedChild2 = ResultSemanticsOperator.Operate(midpoint,
				child2, SubTreePool, MaxTreeDepth);
			return new[] { mutatedChild1 ? child1 : individual1, mutatedChild2 ? child2 : individual2 };
		}
	}
}
