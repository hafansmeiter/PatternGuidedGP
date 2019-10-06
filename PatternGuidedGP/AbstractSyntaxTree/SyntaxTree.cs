﻿using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class SyntaxTree : ICloneable, IDeepCloneable {
		public TreeNode Root { get; private set; }
		public int Height { get; private set; }

		public SyntaxTree(TreeNode root) {
			Root = root;
			CalculateTreeHeight();
		}

		private void CalculateTreeHeight() {
			Height = GetTreeHeight(Root);
		}

		// return true, if newNode is allowed to be placed at oldNode's position
		// false, otherwise
		public bool ReplaceTreeNode(TreeNode oldNode, TreeNode newNode) {
			bool replaced = true;
			if (oldNode.Parent != null) {
				replaced = oldNode.Parent.ReplaceChild(oldNode, newNode);
			} else {
				Root = newNode;
				newNode.Parent = null;
			}
			if (replaced) {
				CalculateTreeHeight();
			}
			return replaced;
		}

		private int GetTreeHeight(TreeNode node) {
			if (node.IsTerminal) {
				return 1;
			}

			int maxChildHeight = 0;
			foreach (var child in node.Children) {
				int height = GetTreeHeight(child);
				if (height > maxChildHeight) {
					maxChildHeight = height;
				}
			}
			return maxChildHeight + (node.IsContainer ? 0 : 1);	// do not count container levels
		}

		public TreeNode GetRandomNode() {
			var nodesPerLevel = new MultiValueDictionary<int, TreeNode>();
			GetNodeHeights(Root, nodesPerLevel, node => !node.IsContainer);
			return UniformRandomSelect(nodesPerLevel);
		}

		public TreeNode GetRandomNode(Type type) {
			var nodesPerLevel = new MultiValueDictionary<int, TreeNode>();
			GetNodeHeights(Root, nodesPerLevel, node => !node.IsContainer && node.Type == type);
			return UniformRandomSelect(nodesPerLevel);
		}

		private TreeNode UniformRandomSelect(MultiValueDictionary<int, TreeNode> nodesPerLevel) {
			if (nodesPerLevel.Count == 0) {
				return null;
			}
			int levelIndex = RandomValueStore.Instance.GetInt(nodesPerLevel.Count);
			var levelNodes = nodesPerLevel.Values.ElementAt(levelIndex);
			int nodeIndex = RandomValueStore.Instance.GetInt(levelNodes.Count);
			return levelNodes.ElementAt(nodeIndex);
		}

		private int GetNodeHeights(TreeNode node, 
			MultiValueDictionary<int, TreeNode> heights,
			Predicate<TreeNode> addNodePredicate) {
			int nodeHeight = (node.IsContainer ? 0 : 1);	// do not container as nodes
			int maxChildHeight = 0;
			foreach (var child in node.Children) {
				int height = GetNodeHeights(child, heights, addNodePredicate);
				if (height > maxChildHeight) {
					maxChildHeight = height;
				}
			}
			if (addNodePredicate(node)) {
				heights.Add(nodeHeight + maxChildHeight, node);
			}
			return nodeHeight + maxChildHeight;
		}

		public object Clone() {
			SyntaxTree copy = (SyntaxTree)base.MemberwiseClone();
			return copy;
		}

		public object DeepClone() {
			SyntaxTree copy = (SyntaxTree)Clone();
			copy.Root = (TreeNode) Root.DeepClone();
			return copy;
		}

		public override string ToString() {
			return Root.ToString();
		}
	}
}
