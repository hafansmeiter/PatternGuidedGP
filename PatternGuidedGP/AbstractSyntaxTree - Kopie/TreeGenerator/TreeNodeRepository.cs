using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.TreeGenerator {
	class TreeNodeRepository {
		private class SimpleRepository {
			private IList<TreeNode> _constants = new List<TreeNode>();
			private IList<TreeNode> _variables = new List<TreeNode>();
			private IList<TreeNode> _nonTerminals = new List<TreeNode>();
			private IList<TreeNode> _terminals = new List<TreeNode>();
			private IList<TreeNode> _all = new List<TreeNode>();

			private Random _random = new Random();
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
				_all.Add(treeNode);
			}

			public TreeNode GetRandomAny() {
				return _all[_random.Next(_all.Count)].Clone() as TreeNode;
			}

			public TreeNode GetRandomNonTerminal() {
				return _nonTerminals[_random.Next(_nonTerminals.Count)].Clone() as TreeNode;
			}

			public TreeNode GetRandomTerminal() {
				return _terminals[_random.Next(_terminals.Count)].Clone() as TreeNode;
			}
		}

		private SimpleRepository _boolRepository = new SimpleRepository();
		private SimpleRepository _intRepository = new SimpleRepository();
		private SimpleRepository _statementsRepository = new SimpleRepository();
		
		public void Add(params TreeNode[] treeNodes) {
			foreach (var node in treeNodes) {
				if (node.Type == typeof(bool)) {
					_boolRepository.Add(node);
				} else if (node.Type == typeof(int)) {
					_intRepository.Add(node);
				} else if (node.Type == typeof(void)) {
					_statementsRepository.Add(node);
				}
			}
		}
		
		public TreeNode GetRandomAny(Type type) {
			if (type == typeof(bool)) {
				return _boolRepository.GetRandomAny();
			}
			else if (type == typeof(int)) {
				return _intRepository.GetRandomAny();
			}
			else if (type == typeof(void)) {
				return _statementsRepository.GetRandomAny();
			}
			return null;
		}

		public TreeNode GetRandomNonTerminal(Type type) {
			if (type == typeof(bool)) {
				return _boolRepository.GetRandomNonTerminal();
			}
			else if (type == typeof(int)) {
				return _intRepository.GetRandomNonTerminal();
			}
			else if (type == typeof(void)) {
				return _statementsRepository.GetRandomNonTerminal();
			}
			return null;
		}

		public TreeNode GetRandomTerminal(Type type) {
			if (type == typeof(bool)) {
				return _boolRepository.GetRandomTerminal();
			}
			else if (type == typeof(int)) {
				return _intRepository.GetRandomTerminal();
			}
			else if (type == typeof(void)) {
				return _statementsRepository.GetRandomTerminal();
			}
			return null;
		}
	}
}
