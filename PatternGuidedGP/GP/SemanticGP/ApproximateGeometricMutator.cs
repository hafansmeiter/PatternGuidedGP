using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.Pool;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP;
using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.GP.Operators;
using PatternGuidedGP.GP.Problems;
using PatternGuidedGP.GP.SemanticGP;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	class ApproximateGeometricMutator : IMutator, ISemanticOperator {
		public IResultSemanticsOperator ResultSemanticsOperator { get; set; } = new PawlakRandomDesiredOperator();
		public ISemanticSubTreePool SubTreePool { get; set; }
		public Semantics DesiredSemantics { get; set; }
		public int MaxTreeDepth { get; set; }
		public IMutator Fallback { get; set; }

		public IFitnessEvaluator FitnessEvaluator { get; set; }
		public Problem Problem { get; set; }

		public ApproximateGeometricMutator(ISemanticSubTreePool subTreePool, int maxTreeDepth) {
			SubTreePool = subTreePool;
			MaxTreeDepth = MaxTreeDepth;
		}
		
		public bool Mutate(Individual individual) {
			bool triedBackPropagation;
			bool mutated = ResultSemanticsOperator.Operate(DesiredSemantics, individual, 
				SubTreePool, MaxTreeDepth, out triedBackPropagation);

			double fitnessChange = 0;
			if (triedBackPropagation && individual.FitnessEvaluated && FitnessEvaluator != null) {
				double fitness = FitnessEvaluator.Evaluate(individual, Problem).Fitness;
				fitnessChange = fitness - individual.Fitness;
			}

			Statistics.Instance.AddBackpropagationAttemptMutation(triedBackPropagation, fitnessChange);
			if (!triedBackPropagation && Fallback != null) {
				return Fallback.Mutate(individual);
			}
			return mutated;

		}
	}
}
