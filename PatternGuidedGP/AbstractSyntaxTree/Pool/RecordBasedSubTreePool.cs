using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.Pool;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.Pool {
	class RecordBasedSubTreePool : SubTreePoolBase, ISyntaxTreeProvider {
		protected class RecordTreeNodeItem : PoolItem {
			public int Improved { get; set; }
			public int Deteriorated { get; set; }

			public RecordTreeNodeItem(TreeNode node, int improved, int deteriorated)
				: base(node) {
				Improved = improved;
				Deteriorated = deteriorated;
			}

			public override double GetFitness() {
				return Deteriorated / Improved;
			}
		}

		protected override PoolItem CreateItem(TreeNode node, object data) {
			return new RecordTreeNodeItem(node, 1, 1);	// start with fitness 1
		}

		public void UpdateRecord(TreeNode treeNode, bool improved) {
			var item = FindNode(treeNode);
			if (item != null) {
				var recordItem = (RecordTreeNodeItem)item;
				if (improved) {
					recordItem.Improved++;
				} else {
					recordItem.Deteriorated++;
				}
				GetItemsByType(treeNode.Type).Sort();
			}
		}

		public TreeNode GetSyntaxTree(int maxDepth, Type type) {
			var items = GetItemsByType(type).Where(item => item.Node.GetTreeHeight() <= maxDepth).ToList();
			if (items.Count == 0) {
				return null;
			}
			else {
				return GetRandomFromList(items).Node;
			}
		}
	}
}
