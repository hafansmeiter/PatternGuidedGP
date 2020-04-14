using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.SyntaxGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Operators {
	class RandomSubtreeCrossover : CrossoverBase {

		public RandomSubtreeCrossover(int maxTreeDepth) : base(maxTreeDepth) {
		}

		public override IEnumerable<Individual> Cross(Individual individual1, Individual individual2) {
			SyntaxTree tree1 = (SyntaxTree) individual1.SyntaxTree.DeepClone();
			SyntaxTree tree2 = individual2.SyntaxTree;
			TreeNode exchangeNode1 = tree1.GetRandomNode();
			Type type = exchangeNode1.Type;
			TreeNode exchangeNode2 = tree2.GetRandomNode(type);
			if (exchangeNode2 != null) {
				// need to clone exchangeNode2 to avoid circular node connections in the tree
				bool replaced = tree1.ReplaceTreeNode(exchangeNode1, (TreeNode) exchangeNode2.DeepClone());
				if (replaced && tree1.Height <= MaxTreeDepth) {
					return new[] { new Individual(tree1) };
				}
			}
			// return individual 1 unchanged if:
			// - individual 2 does not contain chosen node type
			// - resulting tree too large
			return new[] { individual1 };
		}
	}
}
