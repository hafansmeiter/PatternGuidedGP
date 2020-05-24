using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using System;
using System.Collections.Generic;
using System.Linq;

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

	abstract class SubTreePoolBase : ISubTreePool, ISyntaxTreeProvider {
		public bool DuplicateCheck { get; set; } = true;
		public int MaxSizePerType { get; set; } = 50;
		public int KeepItemsOnRemoveWorst { get; set; } = 25;
		public int MaxDepth { get; set; }

		// current items
		protected List<PoolItem> _boolTreeItems
			= new List<PoolItem>();
		protected List<PoolItem> _intTreeItems
			= new List<PoolItem>();
		protected List<PoolItem> _voidTreeItems
			= new List<PoolItem>();

		// items for next generation
		protected List<PoolItem> _newBoolTreeItems
			= new List<PoolItem>();
		protected List<PoolItem> _newIntTreeItems
			= new List<PoolItem>();
		protected List<PoolItem> _newVoidTreeItems
			= new List<PoolItem>();

		protected SubTreePoolBase(int maxDepth = -1) {
			MaxDepth = maxDepth;
		}

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

		protected virtual List<PoolItem> GetNewItemsByType(Type type) {
			if (type == typeof(bool)) {
				return _newBoolTreeItems;
			}
			else if (type == typeof(int)) {
				return _newIntTreeItems;
			}
			else {
				return _newVoidTreeItems;
			}
		}

		protected PoolItem GetRandomFromList(IList<PoolItem> list) {
			return Selector.DrawFromList(list);
		}

		public virtual bool Add(TreeNode node, object data) {
			if (MaxDepth > 0 && node.GetTreeHeight() > MaxDepth) {
				return false;
			}
			var items = GetNewItemsByType(node.Type);
			if (!items.Exists(it => it.Node.Equals(node))) {
				var item = CreateItem(node, data);
				items.Add(item);
			}
			return true;
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
			items.RemoveRange(KeepItemsOnRemoveWorst, n);
		}

		protected abstract PoolItem CreateItem(TreeNode node, object data);

		public virtual void RemoveWorstItems() {
			RemoveLastItems(_boolTreeItems, _boolTreeItems.Count - KeepItemsOnRemoveWorst);
			RemoveLastItems(_intTreeItems, _intTreeItems.Count - KeepItemsOnRemoveWorst);
			RemoveLastItems(_voidTreeItems, _voidTreeItems.Count - KeepItemsOnRemoveWorst);
		}

		public void Clear() {
			_boolTreeItems.Clear();
			_intTreeItems.Clear();
			_voidTreeItems.Clear();
		}

		public TreeNode GetSyntaxTree(int maxDepth, Type type) {
			// ignore maxDepth
			var items = GetItemsByType(type);//.Where(item => item.Node.GetTreeHeight() <= maxDepth).ToList();
			if (items.Count == 0) {
				return null;
			}
			else {
				return GetRandomFromList(items).Node;
			}
		}

		public void TrimToMaxSize() {
			TrimToMaxSize(typeof(bool));
			TrimToMaxSize(typeof(int));
			TrimToMaxSize(typeof(void));
		}

		public void TrimToMaxSize(Type type) {
			var newItems = GetNewItemsByType(type);
			var currentItems = GetItemsByType(type);
			foreach (var curItem in currentItems) {
				var existingItem = newItems.FirstOrDefault(it => it.Node.Equals(curItem.Node));
				if (existingItem != null) {
					if (curItem.GetFitness() < existingItem.GetFitness()) {
						newItems.Remove(existingItem);
						newItems.Add(curItem);
					}
				} else {
					newItems.Add(curItem);
				}
			}
			currentItems.Clear();
			if (newItems.Count > MaxSizePerType) {
				newItems.Sort();
				currentItems.Clear();

				var selector = new RankBasedPoolItemSelector<PoolItem>();
				while (currentItems.Count < MaxSizePerType && newItems.Count > 0) {
					var item = selector.DrawFromList(newItems);
					var existingItem = FindNode(item.Node);
					if (existingItem == null) {	// no duplicates
						currentItems.Add(item);
					} else if (item.GetFitness() < existingItem.GetFitness()) {
						currentItems.Remove(existingItem);
						currentItems.Add(item);
					}
					newItems.Remove(item);
				}
			} else {
				currentItems.AddRange(newItems);
			}
			currentItems.Sort();
			newItems.Clear();
		}
	}
}
