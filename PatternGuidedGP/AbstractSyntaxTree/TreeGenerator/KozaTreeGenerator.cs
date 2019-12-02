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
					if (maxDepth <= 1) {
						child = SelectTerminalNode(type, filter);
					} else {
						child = SelectNonTerminalNode(type, Math.Max(1, maxDepth), filter);
					}
					if (child == null) {    // no valid child found due to filtering
						child = SelectAnyNode(type, Math.Max(1, maxDepth), filter);
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

		private TreeNode SelectAnyNode(Type type, int maxDepth, TreeNodeFilter filter) {
			return InstructionSetRepository.GetRandomAny(type, maxDepth, filter);
		}

		protected abstract TreeNode SelectTerminalNode(Type type, TreeNodeFilter filter);
		protected abstract TreeNode SelectNonTerminalNode(Type type, int maxDepth, TreeNodeFilter filter);

		private TreeNode GetRootNode(Type type, int maxDepth) {
			return InstructionSetRepository.GetRandomNonTerminal(type, maxDepth, null);
		}
	}
}
