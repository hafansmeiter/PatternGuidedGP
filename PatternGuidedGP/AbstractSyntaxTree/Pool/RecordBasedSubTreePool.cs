using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.Pool {
	class RecordBasedSubTreePool : SubTreePoolBase {
		protected class RecordTreeNodeItem : PoolItem {
			// double type, because score may be decreased (e.g. halvened) over generations
			public double Improved { get; set; }
			public double Deteriorated { get; set; }

			public RecordTreeNodeItem(TreeNode node, int improved, int deteriorated)
				: base(node) {
				Improved = improved;
				Deteriorated = deteriorated;
			}

			public override double GetFitness() {
				return Deteriorated / Improved;
			}

			public void DecreaseScore() {
				Improved /= 2;
				Deteriorated /= 2;
			}
		}

		protected override PoolItem CreateItem(TreeNode node, object data) {
			return new RecordTreeNodeItem(node, 1, 1);	// start with fitness 1
		}

		public override void GenerationFinished() {
			base.GenerationFinished();
			foreach (var item in _boolTreeItems) {
				var recordItem = item as RecordTreeNodeItem;
				recordItem.DecreaseScore();

			} 
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
	}
}
