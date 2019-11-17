using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.Pool;
using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.GP.Problems;

namespace PatternGuidedGP.GP.SemanticGP {

	[Obsolete]
	class FitnessEvaluatingRandomDesiredOperator : PawlakRandomDesiredOperator {

		public IFitnessEvaluator FitnessEvaluator { get; set; }
		public Problem Problem { get; set; }

		protected override TreeNode DoLibrarySearch(Individual individual, ISemanticSubTreePool subTreePool, 
			Type nodeType, CombinatorialSemantics desiredSemantics) {
			var treeNode = base.DoLibrarySearch(individual, subTreePool, nodeType, desiredSemantics);
			var recordSubTreePool = subTreePool as RecordBasedSubTreePool;
			if (recordSubTreePool != null) {
				double fitness = FitnessEvaluator.Evaluate(individual, Problem);
				// treat equal fitness like worse fitness => emphasize on improvements
				bool improved = fitness < individual.Fitness;
				recordSubTreePool.UpdateRecord(treeNode, improved);
			}
			return treeNode;
		}
	}
}
