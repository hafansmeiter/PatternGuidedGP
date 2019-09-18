using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.TreeGenerator {
	abstract class KozaTreeGenerator : ITreeGenerator {
		protected TreeNodeRepository _treeNodeRepository;

		public void setTreeNodeRepository(TreeNodeRepository repository) {
			_treeNodeRepository = repository;
		}

		public SyntaxTree GenerateTree(int maxDepth) {
			var root = GetRootNode();
			AddChildren(root, maxDepth - 1);
			return new SyntaxTree(root);
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

		private TreeNode GetRootNode() {
			return _treeNodeRepository.GetRandomNonTerminal(typeof(bool));
		}
	}
}
