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
using PatternGuidedGP.Util;

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
				bool replaced = tree.ReplaceTreeNode(exchangeNode, (TreeNode) newNode.DeepClone());
				if (replaced && tree.Height <= MaxTreeDepth) {
					individual.SyntaxTree = tree;
					if (individual.FitnessEvaluated) {	// only meaningful if fitness of individual is already evaluated
						// update the node's record
						var recordSubTreePool = SyntaxTreeProvider as RecordBasedSubTreePool;
						if (recordSubTreePool != null && FitnessEvaluator != null) {
							var fitness = FitnessEvaluator.Evaluate(individual, Problem).Fitness;

							double improvement = fitness - individual.Fitness;
							Statistics.Instance.AddRecordReplaceAttempt(improvement);
							recordSubTreePool.UpdateRecord(newNode, improvement);
						}
					}
					return true;
				}
			}
			return false;
		}
	}
}
