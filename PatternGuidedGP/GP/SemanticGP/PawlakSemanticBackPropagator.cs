using PatternGuidedGP.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	class PawlakSemanticBackPropagator : ISemanticPropagator {
		// Pawlak et al. Algorithm 1 in paper Semantic Backpropagation for Designing Search Operators in GP:
		// Naming: n ... node, p ... root, t ... target
		// semantic types may be mixed up in expression: e.g. bool ret = (a + b) == (c - a)
		// => have to use object type instead of one generic type
		public CombinatorialSemantics Propagate(TreeNode root, TreeNode node, Semantics target) {
			CombinatorialSemantics semantics = new CombinatorialSemantics(target.Length);
			var path = root.GetPathTo(node);
			for (int i = 0; i < target.Length; i++) {   // for all ti element of t do:
				ISet<object> currentValueSet = new HashSet<object>();   // Di
				currentValueSet.Add(target[i]); // Di <- { ti }
				var currentNode = root;         // a <- p
				bool ambiguityFound = false;    // * not element of Di
				int pathIndex = 0;              // index of path element
				while (currentNode != node && currentValueSet.Count > 0 && !ambiguityFound) {
					int k = currentNode.Children.IndexOf(path[pathIndex]);

					ISet<object> valueSet = new HashSet<object>();  // D' <- {}
					var invertibleExpr = currentNode as IInvertible;    // if not invertible, loop will end
					if (invertibleExpr != null && invertibleExpr.IsInvertible) {
						foreach (var desiredValue in currentValueSet) {
							bool ambiguous;
							var complementValue = invertibleExpr.GetComplementValue(k, i);
							if (complementValue != null) {	// complement value can be null if semantics not evaluated
								valueSet.UnionWith(invertibleExpr.Invert(desiredValue, k, complementValue, out ambiguous));
								if (ambiguous) {
									ambiguityFound = true;
								}
							}
						}
					}
					// valueSet is empty in case of ambiguity (* element of D')
					currentValueSet = valueSet; // Di <- D'
					currentNode = path[pathIndex++];    // a <- Child(a, n)
				}
				semantics[i] = currentValueSet;
			}
			return semantics;
		}
	}
}
