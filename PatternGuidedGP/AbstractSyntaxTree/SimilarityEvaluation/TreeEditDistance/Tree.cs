using PatternGuidedGP.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.SimilarityEvaluation.TreeEditDistance {
	// Based on Java implementation: https://github.com/ijkilchenko/ZhangShasha
	// Publication: https://pdfs.semanticscholar.org/277f/f0c74cc72663d0aabbeae25a3e97b245457c.pdf?_ga=2.230165084.1186213.1577988064-575023686.1577988064
	class Tree {
		Node root = new Node();
		// function l() which gives the leftmost child
		IList<int> l = new List<int>();
		// list of keyroots, i.e., nodes with a left child and the tree root
		IList<int> keyroots = new List<int>();
		// list of the labels of the nodes used for node comparison
		IList<string> labels = new List<string>();

		public Tree(SyntaxTree syntaxTree) {
			root = new Node("root");
			var syntaxRoots = syntaxTree.RootNodes;
			AddChildren(root, syntaxRoots);
		}

		private void AddChildren(Node node, IList<TreeNode> children) {
			foreach (var child in children) {
				var childNode = new Node(child.Description);
				node.Children.Add(childNode);
				AddChildren(childNode, child.Children);
			}
		}

		// the following constructor handles preorder notation. E.g., f(a b(c))
		public Tree(string s) {
			var chars = s.ToCharArray();
			int index = 0;
			root = ParseString(root, chars, ref index);
			if (index < chars.Length - 1) {
				throw new Exception("Invalid tree string.");
			}
		}

		private static Node ParseString(Node node, char[] chars, ref int index) {
			node.Label = chars[index].ToString();
			index++;
			if (index < chars.Length && chars[index] == '(') {
				index++;
				do {
					node.Children.Add(ParseString(new Node(), chars, ref index));
				} while (chars[index] != ')');
				index++;
			}
			return node;
		}

		public void Traverse() {
			// put together an ordered list of node labels of the tree
			Traverse(root, labels);
		}

		private static IList<string> Traverse(Node node, IList<string> labels) {
			for (int i = 0; i < node.Children.Count; i++) {
				labels = Traverse(node.Children[i], labels);
			}
			labels.Add(node.Label);
			return labels;
		}

		public void Index() {
			// index each node in the tree according to traversal method
			Index(root, 0);
		}

		private static int Index(Node node, int index) {
			for (int i = 0; i < node.Children.Count; i++) {
				index = Index(node.Children[i], index);
			}
			index++;
			node.Index = index;
			return index;
		}

		public void L() {
			// put together a function which gives l()
			LeftMost();
			l = L(root, new List<int>());
		}

		private IList<int> L(Node node, IList<int> l) {
			for (int i = 0; i < node.Children.Count; i++) {
				l = L(node.Children[i], l);
			}
			l.Add(node.LeftMost.Index);
			return l;
		}

		private void LeftMost() {
			LeftMost(root);
		}

		private static void LeftMost(Node node) {
			if (node == null)
				return;
			for (int i = 0; i < node.Children.Count; i++) {
				LeftMost(node.Children[i]);
			}
			if (node.Children.Count == 0) {
				node.LeftMost = node;
			}
			else {
				node.LeftMost = node.Children[0].LeftMost;
			}
		}

		public void KeyRoots() {
			// calculate the keyroots
			for (int i = 0; i < l.Count; i++) {
				int flag = 0;
				for (int j = i + 1; j < l.Count; j++) {
					if (l[j] == l[i]) {
						flag = 1;
					}
				}
				if (flag == 0) {
					keyroots.Add(i + 1);
				}
			}
		}

		static int[,] TD;

		public static int ZhangShasha(Tree tree1, Tree tree2) {
			tree1.Index();
			tree1.L();
			tree1.KeyRoots();
			tree1.Traverse();
			tree2.Index();
			tree2.L();
			tree2.KeyRoots();
			tree2.Traverse();

			IList<int> l1 = tree1.l;
			IList<int> keyroots1 = tree1.keyroots;
			IList<int> l2 = tree2.l;
			IList<int> keyroots2 = tree2.keyroots;

			// space complexity of the algorithm
			TD = new int[l1.Count + 1, l2.Count + 1];

			// solve subproblems
			for (int i1 = 1; i1 < keyroots1.Count + 1; i1++) {
				for (int j1 = 1; j1 < keyroots2.Count + 1; j1++) {
					int i = keyroots1[i1 - 1];
					int j = keyroots2[j1 - 1];
					TD[i, j] = treedist(l1, l2, i, j, tree1, tree2);
				}
			}

			return TD[l1.Count, l2.Count];
		}

		private static int treedist(IList<int> l1, IList<int> l2, int i, int j, Tree tree1, Tree tree2) {
			int[,] forestdist = new int[i + 1, j + 1];

			// costs of the three atomic operations
			int Delete = 1;
			int Insert = 1;
			int Relabel = 1;

			forestdist[0,0] = 0;
			for (int i1 = l1[i - 1]; i1 <= i; i1++) {
				forestdist[i1,0] = forestdist[i1 - 1,0] + Delete;
			}
			for (int j1 = l2[j - 1]; j1 <= j; j1++) {
				forestdist[0,j1] = forestdist[0, j1 - 1] + Insert;
			}
			for (int i1 = l1[i - 1]; i1 <= i; i1++) {
				for (int j1 = l2[j - 1]; j1 <= j; j1++) {
					int i_temp = (l1[i - 1] > i1 - 1) ? 0 : i1 - 1;
					int j_temp = (l2[j - 1] > j1 - 1) ? 0 : j1 - 1;
					if ((l1[i1 - 1] == l1[i - 1]) && (l2[j1 - 1] == l2[j - 1])) {

						int Cost = (tree1.labels[i1 - 1].Equals(tree2.labels[j1 - 1])) ? 0 : Relabel;
						forestdist[i1,j1] = Math.Min(
								Math.Min(forestdist[i_temp,j1] + Delete, forestdist[i1,j_temp] + Insert),
								forestdist[i_temp, j_temp] + Cost);
						TD[i1,j1] = forestdist[i1,j1];
					}
					else {
						int i1_temp = l1[i1 - 1] - 1;
						int j1_temp = l2[j1 - 1] - 1;

						int i_temp2 = (l1[i - 1] > i1_temp) ? 0 : i1_temp;
						int j_temp2 = (l2[j - 1] > j1_temp) ? 0 : j1_temp;

						forestdist[i1,j1] = Math.Min(
								Math.Min(forestdist[i_temp,j1] + Delete, forestdist[i1,j_temp] + Insert),
								forestdist[i_temp2,j_temp2] + TD[i1,j1]);
					}
				}
			}
			return forestdist[i,j];
		}
	}
}
