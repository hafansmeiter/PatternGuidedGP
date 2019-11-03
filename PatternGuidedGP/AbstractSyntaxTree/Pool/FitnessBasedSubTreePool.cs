using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.Pool {
	class FitnessBasedSubTreePool : SubTreePoolBase {
		protected class FitnessTreeNodeItem : PoolItem {
			public double Fitness { get; }

			public FitnessTreeNodeItem(TreeNode node, double fitness) 
				: base(node) {
				Fitness = fitness;
			}

			public override double GetFitness() {
				return Fitness;
			}
		}

		protected override PoolItem CreateItem(TreeNode node, object data) {
			return new FitnessTreeNodeItem(node, (double) data);
		}
	}
}
