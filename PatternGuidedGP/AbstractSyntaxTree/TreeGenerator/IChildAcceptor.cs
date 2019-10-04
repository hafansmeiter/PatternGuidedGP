using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.TreeGenerator {
	interface IChildAcceptor {
		bool AcceptChild(TreeNode child, int index);
	}
}
