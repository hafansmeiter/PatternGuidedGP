using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.Pool {
	class FitnessBasedSubTreePool : DefaultSubTreePool {
		protected class FitnessTreeNodeItem : TreeNodeItem {
			public double Fitness { get; }

			public FitnessTreeNodeItem(TreeNode node, double fitness) 
				: base(node) {
				Fitness = fitness;
			}

			public override double GetFitness() {
				return Fitness;
			}
		}

		protected override TreeNodeItem CreateItem(TreeNode node, double fitness) {
			return new FitnessTreeNodeItem(node, fitness);
		}
	}
}
