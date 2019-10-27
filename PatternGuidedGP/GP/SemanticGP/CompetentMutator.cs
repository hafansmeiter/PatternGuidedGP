using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.Pool;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP;
using PatternGuidedGP.GP.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GPExtensions {
	class CompetentMutator : IMutator {
		public ISubTreePool SubTreePool { get; set; }

		public CompetentMutator(ISubTreePool subTreePool) {
			SubTreePool = subTreePool;
		}
		
		public bool Mutate(Individual individual) {
			SyntaxTree tree = (SyntaxTree)individual.SyntaxTree.DeepClone();
			TreeNode exchangeNode = tree.GetRandomNode();
			Type nodeType = exchangeNode.Type;

			TreeNode root;
			if (exchangeNode.IsBackPropagable(out root)) {
				

			}
			return false;
		}
	}
}
