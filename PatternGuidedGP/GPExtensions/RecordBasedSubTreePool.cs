using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GPExtensions {
	class RecordBasedSubTreePool : SubTreePoolBase {
		protected class RecordTreeNodeItem : PoolItem {
			public int Improved { get; private set; }
			public int Deteriorated { get; private set; }

			public RecordTreeNodeItem(TreeNode node)
				: this(node, 0, 0) {
			}

			public RecordTreeNodeItem(TreeNode node, int improved, int deteriorated)
				: base(node) {
				Improved = improved;
				Deteriorated = deteriorated;
			}

			public override double GetFitness() {
				return (double) Improved / (double) Deteriorated;
			}
		}

		protected override PoolItem CreateItem(TreeNode node, object data) {
			return new RecordTreeNodeItem(node, 0, 0);
		}
	}
}
