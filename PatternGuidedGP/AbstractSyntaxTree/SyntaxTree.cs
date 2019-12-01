using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.Pangea;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class SyntaxTree : ICloneable, IDeepCloneable {
		public List<TreeNode> RootNodes { get; private set; } = new List<TreeNode>();
		public int Height => _heights.Max();

		private List<int> _heights = new List<int>();

		public SyntaxTree(params TreeNode[] nodes) {
			RootNodes = nodes.ToList();
			foreach (var node in nodes) {
				_heights.Add(CalculateTreeHeight(node));
			}
		}

		public void AddRootNode(TreeNode node) {
			RootNodes.Add(node);
			_heights.Add(CalculateTreeHeight(node));
		}

		private int CalculateTreeHeight(TreeNode node) {
			return GetTreeHeight(node);
		}

		public IEnumerable<TreeNode> GetTraceableNodes() {
			return GetTreeNodes().Where(node => node is ITraceable && ((ITraceable)node).IsTraceable);
		}

		public TreeNode FindNodeById(ulong id) {
			return GetTreeNodes().Where(node => node.Id == id).SingleOrDefault();
		}

		// return true, if newNode is allowed to be placed at oldNode's position
		// false, otherwise
		public bool ReplaceTreeNode(TreeNode oldNode, TreeNode newNode) {
			bool replaced = true;
			if (oldNode.Parent != null) {
				replaced = oldNode.Parent.ReplaceChild(oldNode, newNode);
				if (replaced) {
					var root = GetRootNode(newNode);
					var index = RootNodes.IndexOf(root);
					_heights[index] = CalculateTreeHeight(root);
				}
			} else {
				var index = RootNodes.IndexOf(oldNode);
				if (index >= 0) {
					RootNodes[index] = newNode;
					newNode.Parent = null;
					_heights[index] = CalculateTreeHeight(newNode);
				}
			}
			return replaced;
		}

		private TreeNode GetRootNode(TreeNode node) {
			var root = node;
			while (root.Parent != null) {
				root = root.Parent;
			}
			return root;
		}

		private int GetTreeHeight(TreeNode node) {
			return node.GetTreeHeight();
		}

		public TreeNode GetRandomNode() {
			var nodesPerLevel = new MultiValueDictionary<int, TreeNode>();
			foreach (var root in RootNodes) {
				GetNodeHeights(root, nodesPerLevel, _ => true, 0);
			}
			return UniformRandomSelect(nodesPerLevel);
		}

		public TreeNode GetRandomNode(Type type) {
			var nodesPerLevel = new MultiValueDictionary<int, TreeNode>();
			foreach (var root in RootNodes) {
				GetNodeHeights(root, nodesPerLevel, node => node.Type == type, 0);
			}
			return UniformRandomSelect(nodesPerLevel);
		}

		private TreeNode UniformRandomSelect(MultiValueDictionary<int, TreeNode> nodesPerLevel) {
			if (nodesPerLevel.Count == 0) {
				return null;
			}
			int levelIndex = RandomValueGenerator.Instance.GetInt(nodesPerLevel.Count);
			var levelNodes = nodesPerLevel.Values.ElementAt(levelIndex);
			int nodeIndex = RandomValueGenerator.Instance.GetInt(levelNodes.Count);
			return levelNodes.ElementAt(nodeIndex);
		}

		public SyntaxNode GetSyntaxNode() {
			return SyntaxFactory.Block(RootNodes.Select(node => (StatementSyntax) node.GetSyntaxNode()));
		}

		private void GetNodeHeights(TreeNode node, 
			MultiValueDictionary<int, TreeNode> heights,
			Predicate<TreeNode> addNodePredicate,
			int height) {
			foreach (var child in node.Children) {
				GetNodeHeights(child, heights, addNodePredicate, height + 1);
			}
			if (addNodePredicate(node)) {
				heights.Add(height, node);
			}
		}

		public IEnumerable<TreeNode> GetTreeNodes() {
			var list = new List<TreeNode>();
			foreach (var node in RootNodes) {
				list.AddRange(node.GetSubTreeNodes(true));
			}
			return list;
		}

		public object Clone() {
			SyntaxTree copy = (SyntaxTree)base.MemberwiseClone();
			return copy;
		}

		public object DeepClone() {
			SyntaxTree copy = (SyntaxTree)Clone();
			copy.RootNodes = RootNodes.ConvertAll(node => (TreeNode) node.DeepClone());
			copy._heights = new List<int>(_heights);
			return copy;
		}

		public override string ToString() {
			StringBuilder builder = new StringBuilder();
			foreach (var node in RootNodes) {
				builder.Append(node.ToString());
			}
			return builder.ToString();
		}

		public override bool Equals(object obj) {
			var tree = obj as SyntaxTree;
			if (tree == null) {
				return false;
			}
			if (tree.RootNodes.Count != RootNodes.Count) {
				return false;
			}
			for (int i = 0; i < RootNodes.Count; i++) {
				if (!RootNodes[i].Equals(tree.RootNodes[i])) {
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode() {
			int hashCode = -1490287827;
			foreach (var node in RootNodes) {
				hashCode += EqualityComparer<TreeNode>.Default.GetHashCode(node);
			}
			return hashCode;
		}
	}
}
