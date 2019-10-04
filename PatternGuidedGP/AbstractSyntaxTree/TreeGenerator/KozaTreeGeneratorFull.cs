using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.TreeGenerator {
	class KozaTreeGeneratorFull : KozaTreeGenerator {

		protected override TreeNode SelectNonTerminalNode(Type type, int maxDepth) {
			return TreeNodeRepository.GetRandomNonTerminal(type, maxDepth);
		}

		protected override TreeNode SelectTerminalNode(Type type) {
			return TreeNodeRepository.GetRandomTerminal(type);
		}
	}
}
