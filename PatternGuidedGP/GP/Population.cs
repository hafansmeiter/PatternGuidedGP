using PatternGuidedGP.AbstractSyntaxTree.SimilarityEvaluation;
using PatternGuidedGP.AbstractSyntaxTree.SimilarityEvaluation.TreeEditDistance;
using PatternGuidedGP.Util;
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
		public int IndividualCount { get; private set; } = 0;
		public bool IsFull { get => IndividualCount == Size; }

		public bool AllowDuplicates { get; set; }

		private Individual[] _individuals;

		public Individual this[int index] {
			get => _individuals[index];
		}

		public Population(int size, bool allowDuplicates = true) {
			Size = size;
			AllowDuplicates = allowDuplicates;
			_individuals = new Individual[size];
		}

		public int Add(params Individual[] individuals) {
			int added = 0;
			for (int i = 0; i < individuals.Length && IndividualCount < Size; i++) {
				if (AllowDuplicates || !ContainsIndividual(individuals[i])) {
					_individuals[IndividualCount++] = individuals[i];
					added++;
				}
			}
			return added;
		}

		public Individual GetFittest() {
			return _individuals[0];
		}

		public IEnumerable<Individual> GetFittest(int n) {
			return _individuals.Take(n);
		}

		public Individual GetRandom() {
			return _individuals[RandomValueGenerator.Instance.GetInt(Size)];
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

		public bool ContainsIndividual(Individual individual) {
			for (int i = 0; i < IndividualCount; i++) {
				if (individual.Equals(_individuals[i])) {
					return true;
				}
			}
			return false;
		}

		public double GetDiversity(ITreeSimilarityMeasure similarityMeasure) {
			int[] distanceSums = new int[IndividualCount];
			for (int i = 0; i < IndividualCount; i++) {
				for (int j = i + 1; j < IndividualCount; j++) {
					int dist = similarityMeasure.Measure(_individuals[i].SyntaxTree, _individuals[j].SyntaxTree);
					distanceSums[i] += dist;
					distanceSums[j] += dist;
				}
			}

			double[] avgDistances = new double[IndividualCount];
			for (int i = 0; i < IndividualCount; i++) {
				avgDistances[i] = distanceSums[i] / (IndividualCount - 1.0);
			}
			return avgDistances.Average();
		}
	}
}
