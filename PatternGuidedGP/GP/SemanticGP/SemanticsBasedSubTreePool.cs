using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	class SemanticsBasedSubTreePool : FitnessBasedSubTreePool, ISemanticSubTreePool {

		public delegate double DistanceMeasure(Semantics semantics1, Semantics semantics2);

		public TreeNode GetBySemantics(Type type, CombinatorialSemantics semantics) {
			var items = GetItemsByType(type);
			if (items.Count > 0) {
				var cartesianProduct = semantics.CartesianProduct();
				if (type == typeof(bool)) {
					return GetBestItem(items, cartesianProduct, Semantics.HammingDistance).Node;
				} else {
					return GetBestItem(items, cartesianProduct, Semantics.NumericDistance).Node;
				}
			}
			return null;
		}

		private PoolItem GetBestItem(IList<PoolItem> items, IEnumerable<Semantics> semantics, DistanceMeasure distanceMeasure) {
			double bestDistance = Double.MaxValue;
			PoolItem bestItem = null;
			foreach (var item in items) {
				var semanticsNode = item.Node as ISemanticsProvider;
				if (semanticsNode != null) {
					foreach (var sem in semantics) {
						var distance = distanceMeasure(semanticsNode.Semantics, sem);
						if (distance < bestDistance) {
							bestDistance = distance;
							bestItem = item;
						}
					}
				}
			}
			return bestItem;
		}
	}
}
