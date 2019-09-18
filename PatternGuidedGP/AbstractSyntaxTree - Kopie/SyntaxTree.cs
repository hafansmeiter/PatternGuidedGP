using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class SyntaxTree {
		public TreeNode Root { get; }

		public SyntaxTree(TreeNode root) {
			Root = root;
		}
	}
}
