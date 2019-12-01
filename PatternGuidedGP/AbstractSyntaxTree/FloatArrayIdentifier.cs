using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class FloatArrayIdentifier : ArrayIdentifier<float> {
		public FloatArrayIdentifier(string name, bool assignable = true) : base(name, assignable) {
		}
	}
}
