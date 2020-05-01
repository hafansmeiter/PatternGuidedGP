using PatternGuidedGP.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	interface ISemanticBackPropagator {
		CombinatorialSemantics Propagate(TreeNode root, TreeNode node, Semantics target);
	}
}
