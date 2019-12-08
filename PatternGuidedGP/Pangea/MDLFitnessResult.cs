using PatternGuidedGP.GP.Evaluators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Pangea {
	class MDLFitnessResult : FitnessResult {
		public MLDataset Dataset { get; private set; }
		public int ClassificationError { get; set; }
		public int TreeSize { get; set; }

		public MDLFitnessResult(double fitness, MLDataset dataset = null)
			: base(fitness) {
			Dataset = dataset;
		}
	}
}
