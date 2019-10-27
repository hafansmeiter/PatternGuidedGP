using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	interface IInvertible {
		bool IsInvertible { get; }
		object GetComplementValue(int k, int semanticsIndex);
		IEnumerable<object> Invert(object desiredValue, int k, object complementValue, out bool ambiguous);
	}
}
