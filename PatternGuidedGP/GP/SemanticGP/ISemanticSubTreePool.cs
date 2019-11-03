using PatternGuidedGP.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	interface ISemanticSubTreePool {
		TreeNode GetBySemantics(Type type, CombinatorialSemantics semantics);
	}
}
