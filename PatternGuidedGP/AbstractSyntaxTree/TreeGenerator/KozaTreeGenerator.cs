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
			if (maxDepth == 0) {
				return;
			}
			for (int i = 0; i < node.ChildTypes.Length; i++) {
				Type type = node.ChildTypes[i];
				TreeNode child;
				do {
					var filter = node.GetChildSelectionFilter(i);
					if (filter != null) {
						child = InstructionSetRepository.GetRandomAny(type, maxDepth, filter);
					} else {
						if (maxDepth == 1) {
							child = SelectTerminalNode(type);
						} else {
							child = SelectNonTerminalNode(type, maxDepth);
						}
					}
					if (node.AcceptChild(child, i)) {
						break;
					}
				} while (true);
				node.AddChild(child);
				// Math.Max: ensure also terminal nodes can have children
				// e.g. Arrays require and index for access
				AddChildren(child, Math.Max(maxDepth - 1, 1));		
			}
		}

		protected abstract TreeNode SelectTerminalNode(Type type);
		protected abstract TreeNode SelectNonTerminalNode(Type type, int maxDepth);

		private TreeNode GetRootNode(Type type, int maxDepth) {
			return InstructionSetRepository.GetRandomNonTerminal(type, maxDepth);
		}
	}
}
