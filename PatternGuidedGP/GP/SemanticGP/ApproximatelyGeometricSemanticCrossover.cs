using PatternGuidedGP.GP;
using PatternGuidedGP.GP.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	class ApproximatelyGeometricSemanticCrossover : ICrossover {

		public IMidpointCalculator MidpointCalculator { get; set; }
		public ISemanticSubTreePool SubTreePool { get; set; }
		public IResultSemanticsOperator ResultSemanticsOperator { get; set; } = new RandomDesiredOperator();
		public int MaxTreeDepth { get; set; }

		public ApproximatelyGeometricSemanticCrossover(ISemanticSubTreePool subTreePool, IMidpointCalculator midpointCalculator, int maxTreeDepth) {
			MidpointCalculator = midpointCalculator;
			SubTreePool = SubTreePool;
			MaxTreeDepth = maxTreeDepth;
		}

		public IEnumerable<Individual> cross(Individual individual1, Individual individual2) {
			var midpoint = MidpointCalculator.GetMidpoint(individual1.Semantics, individual2.Semantics);

			var child1 = new Individual(individual1);
			var child2 = new Individual(individual2);
			bool mutatedChild1 = ResultSemanticsOperator.Operate(midpoint,
				child1, SubTreePool, MaxTreeDepth);
			bool mutatedChild2 = ResultSemanticsOperator.Operate(midpoint,
				child2, SubTreePool, MaxTreeDepth);
			return new[] { mutatedChild1 ? child1 : individual1, mutatedChild2 ? child2 : individual2 };
		}
	}
}
