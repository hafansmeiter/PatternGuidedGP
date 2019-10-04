using PatternGuidedGP.GP.Operators;
using PatternGuidedGP.GP.Problems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP {
	abstract class AlgorithmBase : IGenerationalAlgorithm {
		public IInitializer Initializer { get; set; }
		public IMutator Mutator { get; set; }
		public ICrossover Crossover { get; set; }
		public ISelector Selector { get; set; }

		public double MutationRate { get; set; }
		public double CrossoverRate { get; set; }	// otherwise take over individuals unchanged
		public int Elitism { get; set; }

		public Population Population { get; set; }
		public int Generations { get; }

		public int MaxTreeDepth { get; set; }

		public AlgorithmBase(int populationSize, int generations) {
			Population = new Population(populationSize);
			Generations = generations;
		}

		public abstract Individual Run(Problem problem);
		public abstract bool IsSolutionFound();
		public abstract Population GetNextGeneration(Population population);
	}
}
