using Microsoft.CodeAnalysis;
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
			SyntaxNode root = individual.Syntax;
			SyntaxNode exchangeNode = root.RandomNode();
			Type nodeType = exchangeNode.GetNodeType();

			SyntaxNode newNode = SyntaxTreeProvider.GetTypedSyntaxTree(MaxMutationTreeDepth, nodeType);
			SyntaxNode newTree = root.ReplaceNode(exchangeNode, newNode);
			if (newTree.GetTreeHeight() <= MaxTreeDepth) {
				individual.Syntax = newTree;
				return true;
			}
			return false;
		}
	}
}
