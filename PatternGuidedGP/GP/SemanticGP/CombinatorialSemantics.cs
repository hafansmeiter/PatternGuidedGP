using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	// Generic class would be preferred
	class CombinatorialSemantics {
		private ISet<object>[] _valueSets;
		public int Length { get; }

		public CombinatorialSemantics(int length) {
			_valueSets = new HashSet<object>[length];
			Length = length;
		}

		public ISet<object> this[int i] {
			get {
				return _valueSets[i];
			}
			set {
				_valueSets[i] = value;
			}
		}

		[Obsolete]
		public IEnumerable<Semantics> CartesianProduct() {
			IEnumerable<IEnumerable<object>> emptyProduct = new[] { Enumerable.Empty<object>() };
			return _valueSets.Aggregate(
				emptyProduct,
				(accumulator, sequence) =>
					from acc in accumulator
					from item in sequence
					select acc.Concat(new[] { item })).Select(enumerable => new Semantics(enumerable));
		}

		public static double NumericDistance(CombinatorialSemantics desired, Semantics candidate, IEnumerable<Semantics> allCandidates) {
			double distance = 0.0;
			for (int i = 0; i < Math.Min(desired.Length, candidate.Length); i++) {
				var semantics = desired[i];
				foreach (var value in semantics) {
					if (candidate[i] == null) {
						// if candidate has no semantic value, take the worst distance of the other candidates
						// if no other candidate has a semantic value, ignore i'th value
						bool distanceFound;
						double worstDistance = GetWorstDistance((int) value, allCandidates, i, out distanceFound);
						if (distanceFound) {
							distance += worstDistance; 
						}
					} else {
						var dist = Math.Abs((int)value - (int)candidate[i]);
						distance += dist;
					}
				}
			}
			return distance;
		}

		private static double GetWorstDistance(int value, IEnumerable<Semantics> allCandidates, int i, out bool distanceFound) {
			var found = false;
			var worst = int.MinValue;
			foreach (var candidate in allCandidates) {
				if (candidate[i] != null) {
					var distance = Math.Abs(((int)candidate[i]) - value);
					if (distance > worst) {
						worst = distance;
					}
					found = true;
				}
			}
			distanceFound = found;
			return worst;
		}

		public static double HammingDistance(CombinatorialSemantics desired, Semantics candidate, IEnumerable<Semantics> allCandidates) {
			double distance = 0.0;
			for (int i = 0; i < Math.Min(desired.Length, candidate.Length); i++) {
				var semantics = desired[i];
				foreach (var value in semantics) {
					if (candidate == null || !value.Equals(candidate[i])) {
						distance++;
					}
				}
			}
			return distance;
		}

		public override string ToString() {
			StringBuilder builder = new StringBuilder();
			builder.Append("[");
			for (int i = 0; i < Length; i++) {
				if (i > 0) {
					builder.Append(", ");
				}
				builder.Append("{");
				for (int j = 0; j < _valueSets[i].Count; j++) {
					if (j > 0) {
						builder.Append(", ");
					}
					builder.Append(_valueSets[i].ElementAt(j).ToString());
				}
				builder.Append("}");
			}
			builder.Append("]");
			return builder.ToString();
		}
	}
}
