using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.Pool {
	abstract class PoolItem : IComparable<PoolItem> {
		public TreeNode Node { get; }

		public PoolItem(TreeNode node) {
			Node = node;
		}

		public abstract double GetFitness();

		public virtual int CompareTo(PoolItem other) {
			var ret = GetFitness().CompareTo(other.GetFitness());
			if (ret == 0) {
				ret = Node.GetSubTreeNodes().Count() - other.Node.GetSubTreeNodes().Count();
			}
			return ret;
		}
	}

	abstract class SubTreePoolBase : ISubTreePool {
		public bool DuplicateCheck { get; set; } = true;
		public int MaxSizePerType { get; set; } = 50;
		public int KeepItemsOnGenerationChange { get; set; } = 25;

		protected List<PoolItem> _boolTreeItems
			= new List<PoolItem>();
		protected List<PoolItem> _intTreeItems
			= new List<PoolItem>();
		protected List<PoolItem> _voidTreeItems
			= new List<PoolItem>();

		public IPoolItemSelector<PoolItem> Selector { get; set; }
			= new RandomPoolItemSelector<PoolItem>();

		public virtual TreeNode GetRandom(Type type) {
			var items = GetItemsByType(type);
			if (items.Count == 0) {
				return null;
			}
			else {
				return GetRandomFromList(items).Node;
			}
		}

		protected virtual List<PoolItem> GetItemsByType(Type type) {
			if (type == typeof(bool)) {
				return _boolTreeItems;
			} else if (type == typeof(int)) {
				return _intTreeItems;
			} else {
				return _voidTreeItems;
			}
		}

		protected PoolItem GetRandomFromList(IList<PoolItem> list) {
			return Selector.DrawFromList(list);
		}

		public bool Add(TreeNode node, object data) {
			if (DuplicateCheck && Contains(node)) {
				return false;
			}
			var item = CreateItem(node, data);
			var items = GetItemsByType(node.Type);
			if (items.Count < MaxSizePerType || items.Last().CompareTo(item) >= 0) {
				items.Add(item);
				items.Sort();
				if (items.Count > MaxSizePerType) {
					items.Remove(items.Last());
				}
				return true;
			}
			return false;
		}

		public bool Contains(TreeNode node) {
			return FindNode(node) != null;
		}

		public PoolItem FindNode(TreeNode node) {
			var items = GetItemsByType(node.Type);
			foreach (var item in items) {
				if (item.Node.Equals(node)) {
					return item;
				}
			}
			return null;
		}

		protected void RemoveLastItems(List<PoolItem> items, int n) {
			if (n <= 0) {
				return;
			}
			items.RemoveRange(KeepItemsOnGenerationChange, n);
		}

		protected abstract PoolItem CreateItem(TreeNode node, object data);

		public virtual void GenerationFinished() {
			RemoveLastItems(_boolTreeItems, _boolTreeItems.Count - KeepItemsOnGenerationChange);
			RemoveLastItems(_intTreeItems, _intTreeItems.Count - KeepItemsOnGenerationChange);
		}

		public void Clear() {
			_boolTreeItems.Clear();
			_intTreeItems.Clear();
		}
	}
}
