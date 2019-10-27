using PatternGuidedGP.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	public static class SemanticBackpropagationExtensions {

		// Pawlak et al. Algorithm 1 in paper Semantic Backpropagation for Designing Search Operators in GP:
		// Naming: n ... node, p ... root, t ... target
		// semantic types may be mixed up in expression: e.g. bool ret = (a + b) == (c - a)
		// => have to use object type instead of one generic type
		static IEnumerable<ISet<object>> DoSemanticBackpropagation(this TreeNode node, TreeNode root, Semantics target) {
			var path = root.GetPathTo(node).ToList();
			for (int i = 0; i < target.Length; i++) {	// for all ti element of t do:
				ISet<object> currentValueSet = new HashSet<object>();	// Di
				currentValueSet.Add(target[i]);	// Di <- { ti }
				var currentExpr = root;			// a <- p
				bool ambiguityFound = false;    // * not element of Di
				int pathIndex = 0;				// index of path element
				while (currentExpr != node && currentValueSet.Any() && !ambiguityFound) {
					int k = currentExpr.Children.IndexOf(path[pathIndex]);

					ISet<object> valueSet = new HashSet<object>();	// D' <- {}
					var invertibleExpr = currentExpr as IInvertible;    // if not invertible, loop will end
					if (invertibleExpr != null && invertibleExpr.IsInvertible) {
						foreach (var desiredValue in currentValueSet) {
							bool ambiguous;
							valueSet.UnionWith(invertibleExpr.Invert(desiredValue, k, i, out ambiguous));
							if (ambiguous) {
								ambiguityFound = true;
							}
						}
					}
					currentValueSet = valueSet;	// Di <- D'

					currentExpr = path[pathIndex++];	// a <- Child(a, n)
				}
				yield return currentValueSet;
			}
		}
	}
}
