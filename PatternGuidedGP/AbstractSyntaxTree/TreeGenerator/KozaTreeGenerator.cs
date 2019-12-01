using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using PatternGuidedGP.AbstractSyntaxTree.SyntaxGenerator.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.TreeGenerator {
	abstract class KozaTreeGenerator : ISyntaxTreeProvider {
		public IInstructionSetRepository InstructionSetRepository { get; set; }

		public TreeNode GetSyntaxTree(int maxDepth, Type type) {
			var root = GetRootNode(type, maxDepth);
			AddChildren(root, maxDepth - 1);
			return root;
		}

		private void AddChildren(TreeNode node, int maxDepth) {
			for (int i = 0; i < node.ChildTypes.Length; i++) {
				Type type = node.ChildTypes[i];
				TreeNode child;
				do {
					var filter = node.GetChildSelectionFilter(i);
					if (filter != null) {	// choose from any, because full tree generation might not be possible due to filtering
						child = InstructionSetRepository.GetRandomAny(type, maxDepth, filter);
					} else if (maxDepth <= 1) {
						child = SelectTerminalNode(type);
					} else {
						child = SelectNonTerminalNode(type, maxDepth);
					}
					if (maxDepth == 0 && child.ChildTypes.Length > 0) {	// array index must be a single terminal if already on max depth
						continue;
					}
					if (node.AcceptChild(child, i)) {
						break;
					}
				} while (true);
				node.AddChild(child);
				AddChildren(child, maxDepth - 1);		
			}
		}

		protected abstract TreeNode SelectTerminalNode(Type type);
		protected abstract TreeNode SelectNonTerminalNode(Type type, int maxDepth);

		private TreeNode GetRootNode(Type type, int maxDepth) {
			return InstructionSetRepository.GetRandomNonTerminal(type, maxDepth);
		}
	}
}
