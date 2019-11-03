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

		public IEnumerable<Semantics> CartesianProduct() {
			IEnumerable<IEnumerable<object>> emptyProduct = new[] { Enumerable.Empty<object>() };
			return _valueSets.Aggregate(
				emptyProduct,
				(accumulator, sequence) =>
					from acc in accumulator
					from item in sequence
					select acc.Concat(new[] { item })).Select(enumerable => new Semantics(enumerable));
		}
	}
}
