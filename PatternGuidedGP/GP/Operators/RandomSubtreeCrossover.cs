using Microsoft.CodeAnalysis;
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

		public override Individual cross(Individual individual1, Individual individual2) {
			SyntaxNode root1 = individual1.Syntax;
			SyntaxNode root2 = individual2.Syntax;
			Type type = root1.GetNodeType();
			SyntaxNode exchangeNode1 = root1.RandomNode();
			SyntaxNode exchangeNode2 = root2.RandomNode(type);
			if (exchangeNode2 != null) {
				SyntaxNode newTree = root1.ReplaceNode(exchangeNode1, exchangeNode2);
				if (newTree.GetTreeHeight() <= MaxTreeDepth) {
					return new Individual(newTree);
				}
			}
			// return individual 1 unchanged if:
			// - individual 2 does not contain chosen node type 
			// - resulting tree too large
			return individual1;
		}
	}
}
