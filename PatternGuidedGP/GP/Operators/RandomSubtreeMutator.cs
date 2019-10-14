using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.SyntaxGenerator;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Operators {
	class RandomSubtreeMutator : MutatorBase {

		public RandomSubtreeMutator(ISyntaxTreeProvider provider, int maxTreeDepth, int maxMutationTreeDepth) 
			: base(provider, maxTreeDepth, maxMutationTreeDepth) {
		}

		public override bool Mutate(Individual individual) {
			SyntaxTree tree = (SyntaxTree) individual.SyntaxTree.DeepClone();
			TreeNode exchangeNode = tree.GetRandomNode();
			Type nodeType = exchangeNode.Type;

			TreeNode newNode = SyntaxTreeProvider.GetSyntaxNode(MaxMutationTreeDepth, nodeType);
			bool replaced = tree.ReplaceTreeNode(exchangeNode, newNode);
			if (replaced && tree.Height <= MaxTreeDepth) {
				individual.SyntaxTree = tree;
				return true;
			}
			return false;
		}
	}
}
