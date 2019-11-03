using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	class SemanticsBasedSubTreePool : FitnessBasedSubTreePool, ISemanticSubTreePool {

		public delegate double DistanceMeasure(CombinatorialSemantics desired, Semantics candidate, IEnumerable<Semantics> allCandidates);

		public TreeNode GetBySemantics(Type type, CombinatorialSemantics semantics) {
			var items = GetItemsByType(type);
			if (items.Count > 0) {
				PoolItem bestItem;
				if (type == typeof(bool)) {
					bestItem = GetBestItem(items, semantics, CombinatorialSemantics.HammingDistance);
				} else {
					bestItem = GetBestItem(items, semantics, CombinatorialSemantics.NumericDistance);
				}
				return bestItem.Node;
			}
			return null;
		}

		private PoolItem GetBestItem(IList<PoolItem> items, CombinatorialSemantics semantics, DistanceMeasure distanceMeasure) {
			double bestDistance = Double.MaxValue;
			PoolItem bestItem = null;
			var allSemantics = items.Where(item => item is ISemanticsProvider && ((ISemanticsProvider) item.Node).SemanticsEvaluated)
				.Select(item => ((ISemanticsProvider)item.Node).Semantics);
			foreach (var item in items) {
				var semanticsNode = item.Node as ISemanticsProvider;
				if (semanticsNode != null && semanticsNode.SemanticsEvaluated) {
					var distance = distanceMeasure(semantics, semanticsNode.Semantics, allSemantics);
					if (distance < bestDistance) {
						bestDistance = distance;
						bestItem = item;
					}
				}
			}
			return bestItem;
		}
	}
}
