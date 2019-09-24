using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP {
	class Population {
		public int Size { get; private set; }
		public IEnumerable<Individual> Individuals {
			get {
				return _individuals;
			}
		}

		private Individual[] _individuals;
		private int _count = 0;
		private Random _random = new Random();

		public Population(int size) {
			Size = size;
			_individuals = new Individual[size];
		}

		public void Add(params Individual[] individuals) {
			for (int i = 0; i < individuals.Length && _count < Size; i++) {
				_individuals[_count++] = individuals[i];
			}
		}

		public Individual GetFittest() {
			return _individuals[0];
		}

		public IEnumerable<Individual> GetFittest(int n) {
			return _individuals.Take(n);
		}

		public Individual GetRandom() {
			return _individuals[_random.Next(Size)];
		}

		public void Sort() {
			Array.Sort(_individuals);
		}

		public double GetAverageFitness() {
			double fitnessSum = 0.0;
			foreach (var individual in _individuals) {
				fitnessSum += individual.Fitness;
			}
			return fitnessSum / Size;
		}

	}
}
