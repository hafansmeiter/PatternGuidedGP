using PatternGuidedGP.GP.Operators;
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

		public Problem Problem { get; }
		public Population Population { get; set; }
		public int Generations { get; }

		public int MaxTreeDepth { get; set; }

		public AlgorithmBase(Problem problem, int populationSize, int generations) {
			Problem = problem;
			Population = new Population(populationSize);
			Generations = generations;
		}

		public abstract Individual Run();
		public abstract bool IsSolutionFound();
		public abstract Population GetNextGeneration(Population population);
	}
}
