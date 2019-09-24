using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Operators {
	abstract class CrossoverBase : ICrossover {
		public int MaxTreeDepth { get; set; }

		public CrossoverBase(int maxTreeDepth) {
			MaxTreeDepth = maxTreeDepth;
		}

		public abstract Individual cross(Individual individual1, Individual individual2);
	}
}
