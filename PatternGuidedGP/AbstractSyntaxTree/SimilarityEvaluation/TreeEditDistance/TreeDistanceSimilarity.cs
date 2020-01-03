using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.SimilarityEvaluation.TreeEditDistance {
	class TreeDistanceSimilarity : ITreeSimilarityMeasure {
		public int Measure(SyntaxTree a, SyntaxTree b) {
			Tree tree1 = new Tree(a);
			Tree tree2 = new Tree(b);

			return Tree.ZhangShasha(tree1, tree2);
		}
	}
}
