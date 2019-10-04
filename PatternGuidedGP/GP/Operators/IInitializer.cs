using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Operators {
	interface IInitializer {
		void Initialize(Population population, int maxTreeDepth, Type rootType);
	}
}
