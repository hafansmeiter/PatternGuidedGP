using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	// Generic class would be preferred
	class Semantics {
		private object[] _values;
		public int Length { get; private set; }

		public Semantics(int length) {
			_values = new object[length];
			Length = length;
		}

		public Semantics(IEnumerable<object> semanticValues) {
			_values = semanticValues.ToArray();
			Length = _values.Length;
		}

		public object this[int i] {
			get {
				return _values[i];
			}
			set {
				_values[i] = value;
			}
		}
		
		public static double NumericDistance(Semantics semantics1, Semantics semantics2) {
			double distance = 0.0;
			for (int i = 0; i < Math.Min(semantics1.Length, semantics2.Length); i++) {
				double s1 = (double)semantics1[i];
				double s2 = (double)semantics2[i];
				distance += s1 - s2;
			}
			return distance;
		}

		public static double HammingDistance(Semantics semantics1, Semantics semantics2) {
			double distance = 0.0;
			for (int i = 0; i < Math.Min(semantics1.Length, semantics2.Length); i++) {
				if (semantics1[i] == null || semantics2[i] == null) {
					if (semantics1[i] == null && semantics2[i] != null ||
						semantics1[i] != null && semantics2[i] == null) {
						distance++;
					}
				} else if (!semantics1[i].Equals(semantics2[i])) {
					distance++;
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
				builder.Append(_values[i].ToString());
			}
			builder.Append("]");
			return builder.ToString();
		}
	}
}
