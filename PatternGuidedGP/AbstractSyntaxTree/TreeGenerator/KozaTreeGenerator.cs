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
		public ITreeNodeRepository TreeNodeRepository { get; set; }

		public SyntaxNode GetSyntaxTree(int maxDepth, Type type) {
			var root = GetRootNode(type, maxDepth);
			AddChildren(root, maxDepth - 1);
			return root.GetSyntaxNode();
		}

		private void AddChildren(TreeNode node, int maxDepth) {
			for (int i = 0; i < node.ChildTypes.Length; i++) {
				Type type = node.ChildTypes[i];
				TreeNode child;
				do {
					if (maxDepth == 1) {
						child = SelectTerminalNode(type);
					} else {
						child = SelectNonTerminalNode(type, maxDepth);
					}
					if (!node.AcceptChild(child, i)) {
						child = null;
					}
				} while (child == null);
				node.Children.Add(child);
				if (maxDepth > 1) {
					AddChildren(child, maxDepth - 1);
				}
			}
		}

		protected abstract TreeNode SelectTerminalNode(Type type);
		protected abstract TreeNode SelectNonTerminalNode(Type type, int maxDepth);

		private TreeNode GetRootNode(Type type, int maxDepth) {
			return TreeNodeRepository.GetRandomNonTerminal(type, maxDepth);
		}
	}
}
