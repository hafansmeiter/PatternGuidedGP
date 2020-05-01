using PatternGuidedGP.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {

	// Pawlak et al. - Semantic Backpropagation for Designing Search Operators in GP: page 332
	// Random Desired Operator (RDO) algorithm to replace random node by semantically fitting subtree
	class PawlakRandomDesiredOperator : IResultSemanticsOperator {
		public ISemanticBackPropagator SemanticBackPropagator { get; set; } = new PawlakSemanticBackPropagator();

		public bool Operate(Semantics resultSemantics, Individual individual, ISemanticSubTreePool subTreePool, 
			int maxTreeDepth, out bool triedBackPropagation) {
			SyntaxTree tree = (SyntaxTree)individual.SyntaxTree.DeepClone();
			TreeNode exchangeNode = tree.GetRandomNode();
			Type nodeType = exchangeNode.Type;
			
			TreeNode root;
			if (exchangeNode.IsBackPropagable(out root)) {
				triedBackPropagation = true;
				var desiredSemantics = DoSemanticBackPropagation(resultSemantics, exchangeNode, root);
				TreeNode newNode = DoLibrarySearch(individual, subTreePool, nodeType, desiredSemantics);

				if (newNode != null) {
					bool replaced = tree.ReplaceTreeNode(exchangeNode, newNode);
					if (replaced && tree.Height <= maxTreeDepth) {
						individual.SyntaxTree = tree;
						return true;
					}
					return false;
				}
			}
			triedBackPropagation = false;
			return false;
		}

		protected virtual CombinatorialSemantics DoSemanticBackPropagation(Semantics resultSemantics, 
			TreeNode exchangeNode, TreeNode root) {
			return SemanticBackPropagator.Propagate(root, exchangeNode, resultSemantics);
		}

		protected virtual TreeNode DoLibrarySearch(Individual individual, ISemanticSubTreePool subTreePool, 
			Type nodeType, CombinatorialSemantics desiredSemantics) {
			return subTreePool.GetBySemantics(nodeType, desiredSemantics);
		}
	}
}
