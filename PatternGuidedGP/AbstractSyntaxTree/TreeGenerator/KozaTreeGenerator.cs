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
		public TreeNodeRepository TreeNodeRepository { get; set; }

		public SyntaxNode GetSyntaxTree(int maxDepth) {
			var root = GetRandomRootNode();
			AddChildren(root, maxDepth - 1);
			return root.GetSyntaxNode();
		}

		public SyntaxNode GetTypedSyntaxTree(int maxDepth, Type type) {
			var root = GetRootNode(type);
			AddChildren(root, maxDepth - 1);
			return root.GetSyntaxNode();
		}

		private void AddChildren(TreeNode node, int maxDepth) {
			/// TODO: Add support for statements.
			foreach (var type in node.ChildTypes) {
				if (maxDepth == 1) {
					var child = SelectTerminalNode(type);
					node.Children.Add(child);
				} else {
					var child = SelectNonTerminalNode(type);
					AddChildren(child, maxDepth - 1);
					node.Children.Add(child);
				}
			}
		}

		protected abstract TreeNode SelectTerminalNode(Type type);
		protected abstract TreeNode SelectNonTerminalNode(Type type);

		private TreeNode GetRootNode(Type type) {
			return TreeNodeRepository.GetRandomNonTerminal(type);
		}

		private TreeNode GetRandomRootNode() {
			return TreeNodeRepository.GetRandomNonTerminal(typeof(bool));
		}
	}
}
