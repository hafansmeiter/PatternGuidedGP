using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	class Semantics {
		private object[] _values;
		public int Length { get; private set; }

		public Semantics(int length) {
			_values = new object[length];
			Length = length;
		}

		public object this[int i] {
			get {
				return _values[i];
			}
			set {
				_values[i] = value;
			}
		}
	}
}
