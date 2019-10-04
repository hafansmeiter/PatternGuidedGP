using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.TreeGenerator {
	class TreeNodeRepository : ITreeNodeRepository {
		private class TreeNodeDictionary {
			private IDictionary<int, List<TreeNode>> _nodeLists = new Dictionary<int, List<TreeNode>>();
			private int _maxDepth = 0;

			public void Add(TreeNode node) {
				int depth = node.RequiredTreeDepth;
				List<TreeNode> nodes;
				if (!_nodeLists.TryGetValue(depth, out nodes)) {
					nodes = new List<TreeNode>();
					_nodeLists.Add(depth, nodes);
				}
				nodes.Add(node);
				if (depth > _maxDepth) {
					_maxDepth = depth;
				}
				PropagateNodes(depth);
			}

			public IList<TreeNode> GetTreeNodes(int maxDepth) {
				int depth = Math.Min(maxDepth, _maxDepth);
				return _nodeLists[depth];
			}

			private void PropagateNodes(int fromDepth) {
				var nodes = _nodeLists[fromDepth];
				for (int i = fromDepth + 1; i <= _maxDepth; i++) {
					List<TreeNode> nextNodes;
					if (!_nodeLists.TryGetValue(i, out nextNodes)) {
						nextNodes = new List<TreeNode>();
						_nodeLists.Add(i, nextNodes);
					}
					nodes = nodes.Union(nextNodes).ToList();
				}
			}
		}

		private class SimpleRepository {
			private IList<TreeNode> _constants = new List<TreeNode>();
			private IList<TreeNode> _variables = new List<TreeNode>();
			private IList<TreeNode> _terminals = new List<TreeNode>();
			private TreeNodeDictionary _nonTerminals = new TreeNodeDictionary();
			private TreeNodeDictionary _any = new TreeNodeDictionary();

			private double _variableRatio = 0.7;

			public void Add(TreeNode treeNode) {
				if (treeNode.IsTerminal) {
					if (treeNode.IsVariable) {
						_variables.Add(treeNode);
					} else {
						_constants.Add(treeNode);
					}
					_terminals.Add(treeNode);
				} else {
					_nonTerminals.Add(treeNode);
				}
				_any.Add(treeNode);
			}

			public TreeNode GetRandomAny(int maxDepth) {
				var nodes = _any.GetTreeNodes(maxDepth);
				var node = nodes[RandomValueStore.Instance.GetInt(nodes.Count)].Clone() as TreeNode;
				node.Initialize();
				return node;
			}

			public TreeNode GetRandomNonTerminal(int maxDepth) {
				var nodes = _nonTerminals.GetTreeNodes(maxDepth);
				var node = nodes[RandomValueStore.Instance.GetInt(nodes.Count)].Clone() as TreeNode;
				node.Initialize();
				return node;
			}

			public TreeNode GetRandomTerminal() {
				var node = _terminals[RandomValueStore.Instance.GetInt(_terminals.Count)].Clone() as TreeNode;
				node.Initialize();
				return node;
			}
		}

		private IDictionary<Type, SimpleRepository> _repositories = new Dictionary<Type, SimpleRepository>();

		public void Add(params TreeNode[] treeNodes) {
			foreach (var node in treeNodes) {
				GetTypeRepository(node.Type).Add(node);
			}
		}
		
		public TreeNode GetRandomAny(Type type, int maxDepth) {
			return GetTypeRepository(type).GetRandomAny(maxDepth);
		}

		public TreeNode GetRandomNonTerminal(Type type, int maxDepth) {
			return GetTypeRepository(type).GetRandomNonTerminal(maxDepth);
		}

		public TreeNode GetRandomTerminal(Type type) {
			return GetTypeRepository(type).GetRandomTerminal();
		}

		private SimpleRepository GetTypeRepository(Type type) {
			SimpleRepository repository;
			if (!_repositories.TryGetValue(type, out repository)) {
				repository = new SimpleRepository();
				_repositories.Add(type, repository);
			}
			return repository;
		}
	}
}
