using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.SimilarityEvaluation.TreeEditDistance {
	// Based on Java implementation: https://github.com/ijkilchenko/ZhangShasha
	// Publication: https://pdfs.semanticscholar.org/277f/f0c74cc72663d0aabbeae25a3e97b245457c.pdf?_ga=2.230165084.1186213.1577988064-575023686.1577988064
	class Node {
		public string Label { get; set; }
		public int Index { get; set; }
		public IList<Node> Children { get; set; } = new List<Node>();
		public Node LeftMost { get; set; }

		public Node() {
		}

		public Node(string label) {
			Label = label;
		}
	}
}
