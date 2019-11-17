using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.GP.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP {
	class RunConfiguration {
		public string Name { get; set; }
		public ICrossover Crossover { get; set; }
		public IMutator Mutator { get; set; }
		public IFitnessEvaluator FitnessEvaluator { get; set; }
		public int Runs { get; set; } = 10;
		public int PopulationSize { get; set; } = 150;
		public int Generations { get; set; } = 100;

		public RunConfiguration(string name) {
			Name = name;
		}
	}
}
