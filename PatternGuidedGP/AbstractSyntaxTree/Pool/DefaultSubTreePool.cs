using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.Pool {
	class TreeNodeItem : IComparable<TreeNodeItem> {
		public TreeNode Node { get; }

		public TreeNodeItem(TreeNode node) {
			Node = node;
		}

		public virtual double GetFitness() {
			// default fitness: tree size
			return Node.GetSubTreeNodes().Count();
		}

		public int CompareTo(TreeNodeItem other) {
			return GetFitness().CompareTo(other.GetFitness());
		}
	}

	class DefaultSubTreePool : ISubTreePool, ISyntaxTreeProvider {

		public bool DuplicateCheck { get; set; } = true;
		public int MaxSize { get; set; } = 50;
		public IPoolItemSelector<TreeNodeItem> Selector { get; set; }
			= new RankBasedPoolItemSelector<TreeNodeItem>();

		private SortedSet<TreeNodeItem> _items 
			= new SortedSet<TreeNodeItem>();

		public bool Add(TreeNode node, double fitness) {
			if (DuplicateCheck && Contains(node)) {
				return false;
			}
			var item = CreateItem(node, fitness);
			if (_items.Count < MaxSize || _items.Last().CompareTo(item) >= 0) {
				_items.Add(item);
				if (_items.Count > MaxSize) {
					_items.Remove(_items.Last());
				}
				return true;
			}
			return false;
		}

		public TreeNode GetRandom(Type type, int maxDepth) {
			var items = _items.Where(item => 
				item.Node.Type == type && 
				item.Node.GetTreeHeight() <= maxDepth).ToList();
			if (items.Count == 0) {
				return null;
			} else {
				return GetRandomFromList(items).Node;
			}
		}

		protected TreeNodeItem GetRandomFromList(IList<TreeNodeItem> list) {
			return Selector.DrawFromList(list);
		}

		protected virtual TreeNodeItem CreateItem(TreeNode node, double fitness) {
			return new TreeNodeItem(node);
		}

		public bool Contains(TreeNode node) {
			foreach (var item in _items) {
				if (item.Node.EqualsTreeNode(node)) {
					return true;
				}
			}
			return false;
		}

		public TreeNode GetSyntaxTree(int maxDepth, Type type) {
			return GetRandom(type, maxDepth);
		}
	}
}
