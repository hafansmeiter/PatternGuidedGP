using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.Pool;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.GP.Problems;

namespace PatternGuidedGP.GP.Operators {
	class EvaluatingRandomSubtreeMutator : RandomSubtreeMutator {
		public IFitnessEvaluator FitnessEvaluator { get; set; }
		public Problem Problem { get; set; }

		public EvaluatingRandomSubtreeMutator(ISyntaxTreeProvider provider, int maxTreeDepth, int maxMutationTreeDepth) : base(provider, maxTreeDepth, maxMutationTreeDepth) {
		}

		public override bool Mutate(Individual individual) {
			SyntaxTree tree = (SyntaxTree)individual.SyntaxTree.DeepClone();
			TreeNode exchangeNode = tree.GetRandomNode();
			Type nodeType = exchangeNode.Type;

			TreeNode newNode = SyntaxTreeProvider.GetSyntaxTree(MaxMutationTreeDepth, nodeType);
			if (newNode != null) {
				bool replaced = tree.ReplaceTreeNode(exchangeNode, newNode);
				if (replaced && tree.Height <= MaxTreeDepth) {
					individual.SyntaxTree = tree;
					// update the node's record
					var recordSubTreePool = SyntaxTreeProvider as RecordBasedSubTreePool;
					if (recordSubTreePool != null && FitnessEvaluator != null) {
						var fitness = FitnessEvaluator.Evaluate(individual, Problem).Fitness;
						if (fitness < individual.Fitness) {
							recordSubTreePool.UpdateRecord(newNode, true);
						} else {
							recordSubTreePool.UpdateRecord(newNode, false);
						}
					}
					return true;
				}
			}
			return false;
		}
	}
}
