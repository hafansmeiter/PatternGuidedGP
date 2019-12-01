using PatternGuidedGP.GP.Evaluators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Pangea {
	class MDLFitnessResult : FitnessResult {
		public MLDataset Dataset { get; private set; }

		public MDLFitnessResult(double fitness, MLDataset dataset)
			: base(fitness) {
			Dataset = dataset;
		}
	}
}
