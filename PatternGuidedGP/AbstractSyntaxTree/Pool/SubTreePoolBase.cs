using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.Pool {
	class PoolItem : IComparable<PoolItem> {
		public TreeNode Node { get; }

		public PoolItem(TreeNode node) {
			Node = node;
		}

		public virtual double GetFitness() {
			return 0.0;
		}

		public int CompareTo(PoolItem other) {
			return GetFitness().CompareTo(other.GetFitness());
		}
	}

	abstract class SubTreePoolBase : ISubTreePool {
		public bool DuplicateCheck { get; set; } = true;
		public int MaxSize { get; set; } = 50;
		protected SortedSet<PoolItem> _items
			= new SortedSet<PoolItem>();

		public IPoolItemSelector<PoolItem> Selector { get; set; }
			= new RankBasedPoolItemSelector<PoolItem>();

		public virtual TreeNode GetRandom(Type type) {
			var items = GetItemsByType(type);
			if (items.Count == 0) {
				return null;
			}
			else {
				return GetRandomFromList(items).Node;
			}
		}

		protected virtual IList<PoolItem> GetItemsByType(Type type) {
			return _items.Where(item => item.Node.Type == type).ToList();
		}

		protected PoolItem GetRandomFromList(IList<PoolItem> list) {
			return Selector.DrawFromList(list);
		}

		public bool Add(TreeNode node, object data) {
			if (DuplicateCheck && Contains(node)) {
				return false;
			}
			var item = CreateItem(node, data);
			if (_items.Count < MaxSize || _items.Last().CompareTo(item) >= 0) {
				_items.Add(item);
				if (_items.Count > MaxSize) {
					_items.Remove(_items.Last());
				}
				return true;
			}
			return false;
		}

		public bool Contains(TreeNode node) {
			foreach (var item in _items) {
				if (item.Node.EqualsTreeNode(node)) {
					return true;
				}
			}
			return false;
		}

		protected abstract PoolItem CreateItem(TreeNode node, object data);
	}
}
