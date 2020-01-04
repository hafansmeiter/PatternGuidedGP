using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.Pool;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.Pool {
	// Item order according to Upper Confidence Bounds (UCB)
	class RecordBasedSubTreePool : SubTreePoolBase, ISyntaxTreeProvider {

		protected class RecordTreeNodeItem : PoolItem {
			public int Improved { get; private set; }
			public int Deteriorated { get; private set; }
			public int Evaluations { get; private set; }
			public int TotalEvaluations { get; set; }
			public double Fitness { get; private set; }

			public RecordTreeNodeItem(TreeNode node, double fitness)
				: base(node) {
				Fitness = fitness;
			}

			public override double GetFitness() {
				double ret = 0;
				if (Evaluations > 0) {
					// Maximize UCB formula:
					// xi + sqrt(2 * ln(N) / ni)
					// xi = average payout (score)
					// N = total number of tries
					// ni = number of tries
					ret = (Improved / Math.Max(0.33, Deteriorated)) +	// avoid division by 0 (question: how to value a 1-0 score? here 3-1)
						Math.Sqrt((2 * Math.Log(TotalEvaluations)) / Evaluations);
				}
				return ret;
			}

			public override int CompareTo(PoolItem other) {
				var otherItem = (RecordTreeNodeItem)other;
				// order items as follows:
				// 1. evaluated items with positive record
				//   .) order by UCB score
				// 2. not evaluated items
				//   .) order by individual fitness
				//   .) order by tree height
				// 3. evaluated items with negative record
				//   .) order by UCB score
				if (Evaluations > 0 && otherItem.Evaluations > 0) {
					return otherItem.GetFitness().CompareTo(GetFitness());	// swap, because UCB formula is maximized
				} else if (Evaluations > 0) {
					return Deteriorated - (Improved + 1);	// prefer equal record to not evaluated
				} else if (otherItem.Evaluations > 0) {
					return (otherItem.Improved + 1) - otherItem.Deteriorated;
				} else {
					int ret = Fitness.CompareTo(otherItem.Fitness);
					if (ret == 0) {
						ret = Node.GetSubTreeNodes().Count() - other.Node.GetSubTreeNodes().Count();
					}
					return ret;
				}
			}

			public void AddEvaluation(bool improved) {
				if (improved) {
					Improved++;
				} else {
					Deteriorated++;
				}
				Evaluations++;
			}
		}

		public RecordBasedSubTreePool(int maxDepth = -1) : base(maxDepth) {
			Selector = new RankBasedPoolItemSelector<PoolItem>();
		}

		protected override PoolItem CreateItem(TreeNode node, object data) {
			return new RecordTreeNodeItem(node, (double) data);
		}

		public void UpdateRecord(TreeNode treeNode, bool improved) {
			var item = FindNode(treeNode);
			Logger.WriteLine(4, "Update record " + improved + " to " + item.Node);
			if (item != null) {
				var recordItem = (RecordTreeNodeItem)item;
				recordItem.AddEvaluation(improved);

				UpdateRecords(treeNode.Type);
			}
		}

		private void UpdateRecords(Type type) {
			var allItems = GetItemsByType(type);
			int totalEvaluations = allItems.Sum(item => ((RecordTreeNodeItem)item).Evaluations);
			allItems.ForEach(item => ((RecordTreeNodeItem)item).TotalEvaluations = totalEvaluations);
			allItems.Sort();
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

		public override bool Add(TreeNode node, object data) {
			var ret = base.Add(node, data);
			if (ret) {
				UpdateRecords(node.Type);
			}
			return ret;
		}

		public override void RemoveWorstItems() {
			/*int i = 0;
			Logger.WriteLine(4, "Record-based pool content for BOOL:");
			foreach (var item in GetItemsByType(typeof(bool))) {
				RecordTreeNodeItem recordItem = (RecordTreeNodeItem)item;
				Logger.WriteLine(4, "#" + i + ": (" + recordItem.Improved + "-" + recordItem.Deteriorated + "; fit=" + recordItem.Fitness + "): " + item.Node.ToString());
				i++;
			}
			i = 0;
			Logger.WriteLine(0, "Record-based pool content for INT:");
			foreach (var item in GetItemsByType(typeof(int))) {
				RecordTreeNodeItem recordItem = (RecordTreeNodeItem)item;
				Logger.WriteLine(0, "#" + i + ": (" + recordItem.Improved + "-" + recordItem.Deteriorated + "; fit=" + recordItem.Fitness + "): " + item.Node.ToString());
				i++;
			}
			i = 0;
			Logger.WriteLine(0, "Record-based pool content for VOID:");
			foreach (var item in GetItemsByType(typeof(void))) {
				RecordTreeNodeItem recordItem = (RecordTreeNodeItem)item;
				Logger.WriteLine(0, "#" + i + ": (" + recordItem.Improved + "-" + recordItem.Deteriorated + "; fit=" + recordItem.Fitness + "): " + item.Node.ToString());
				i++;
			}*/

			base.RemoveWorstItems();
			UpdateRecords(typeof(bool));
			UpdateRecords(typeof(int));
			UpdateRecords(typeof(void));
		}
	}
}
