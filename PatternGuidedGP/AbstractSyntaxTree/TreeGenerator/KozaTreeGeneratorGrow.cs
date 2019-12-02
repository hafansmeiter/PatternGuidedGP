using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.TreeGenerator {
	class KozaTreeGeneratorGrow : KozaTreeGenerator {

		protected override TreeNode SelectNonTerminalNode(Type type, int maxDepth, TreeNodeFilter filter) {
			return InstructionSetRepository.GetRandomAny(type, maxDepth, filter);
		}

		protected override TreeNode SelectTerminalNode(Type type, TreeNodeFilter filter) {
			return InstructionSetRepository.GetRandomTerminal(type, filter);
		}
	}
}
