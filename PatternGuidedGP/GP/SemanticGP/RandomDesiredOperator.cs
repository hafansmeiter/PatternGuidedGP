using PatternGuidedGP.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {

	// Pawlak et al. - Semantic Backpropagation for Designing Search Operators in GP: page 332
	// Random Desired Operator (RDO) algorithm to replace random node by semantically fitting subtree
	class RandomDesiredOperator : IResultSemanticsOperator {
		public ISemanticPropagator SemanticBackPropagator { get; set; }

		public bool Operate(Semantics resultSemantics, Individual individual, ISemanticSubTreePool subTreePool, int maxTreeDepth) {
			SyntaxTree tree = (SyntaxTree)individual.SyntaxTree.DeepClone();
			TreeNode exchangeNode = tree.GetRandomNode();
			Type nodeType = exchangeNode.Type;

			TreeNode root;
			if (exchangeNode.IsBackPropagable(out root)) {
				var desiredSemantics = SemanticBackPropagator.Propagate(root, exchangeNode, resultSemantics);
				TreeNode newNode = subTreePool.GetBySemantics(nodeType, desiredSemantics);

				bool replaced = tree.ReplaceTreeNode(exchangeNode, newNode);
				if (replaced && tree.Height <= maxTreeDepth) {
					individual.SyntaxTree = tree;
					return true;
				}
				return false;
			}
			return false;
		}
	}
}
