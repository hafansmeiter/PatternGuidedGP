using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.TreeGenerator {
	class InstructionSetRepository : IInstructionSetRepository {
		private class TreeNodeDictionary {
			private IDictionary<int, HashSet<TreeNode>> _nodeLists = new Dictionary<int, HashSet<TreeNode>>();
			private int _maxDepth = 0;

			public void Add(TreeNode node) {
				int depth = node.RequiredTreeDepth;
				HashSet<TreeNode> nodes;
				if (!_nodeLists.TryGetValue(depth, out nodes)) {
					nodes = new HashSet<TreeNode>();
					_nodeLists.Add(depth, nodes);
				}
				nodes.Add(node);
				if (depth > _maxDepth) {
					_maxDepth = depth;
				}
				PropagateNodes();
			}

			public IList<TreeNode> GetTreeNodes(int maxDepth) {
				int depth = Math.Min(maxDepth, _maxDepth);
				return _nodeLists[depth].ToList();
			}

			private void PropagateNodes() {
				var prevNodes = new HashSet<TreeNode>();
				for (int i = 0; i <= _maxDepth; i++) {
					HashSet<TreeNode> currentNodes;
					if (!_nodeLists.TryGetValue(i, out currentNodes)) {
						currentNodes = new HashSet<TreeNode>(prevNodes);
						_nodeLists.Add(i, currentNodes);
					} else {
						currentNodes.UnionWith(prevNodes);
					}
					prevNodes = currentNodes;
				}
			}
		}

		private class SimpleRepository {
			private IList<TreeNode> _constants = new List<TreeNode>();
			private IList<TreeNode> _variables = new List<TreeNode>();
			private IList<TreeNode> _terminals = new List<TreeNode>();
			private TreeNodeDictionary _nonTerminals = new TreeNodeDictionary();
			private TreeNodeDictionary _any = new TreeNodeDictionary();

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

			public TreeNode GetRandomAny(int maxDepth, TreeNodeFilter filter = null) {
				var nodes = _any.GetTreeNodes(maxDepth);
				if (filter != null) {
					nodes = filter(nodes).ToList();
				}
				var node = nodes[RandomValueGenerator.Instance.GetInt(nodes.Count)].Clone() as TreeNode;
				node.Initialize();
				return node;
			}

			public TreeNode GetRandomNonTerminal(int maxDepth, TreeNodeFilter filter = null) {
				var nodes = _nonTerminals.GetTreeNodes(maxDepth);
				if (filter != null) {
					nodes = filter(nodes).ToList();
				}
				var node = nodes[RandomValueGenerator.Instance.GetInt(nodes.Count)].Clone() as TreeNode;
				node.Initialize();
				return node;
			}

			public TreeNode GetRandomTerminal(TreeNodeFilter filter = null) {
				var nodes = _terminals; 
				if (filter != null) {
					nodes = filter(nodes).ToList();
				}
				var node = nodes[RandomValueGenerator.Instance.GetInt(nodes.Count)].Clone() as TreeNode;
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
		
		public TreeNode GetRandomAny(Type type, int maxDepth, TreeNodeFilter filter) {
			return GetTypeRepository(type).GetRandomAny(maxDepth, filter);
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
