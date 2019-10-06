using PatternGuidedGP.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP {
	class Individual : IComparable<Individual> {
		public double Fitness {
			get {
				return _fitness;
			}
			set {
				_fitness = value;
				_fitnessEvaluated = true;
			}
		}
		public bool FitnessEvaluated {
			get {
				return _fitnessEvaluated;
			}
			set {
				_fitnessEvaluated = value;
			}
		}
		public SyntaxTree SyntaxTree { get; set; }

		private double _fitness = 0.0;
		private bool _fitnessEvaluated = false;

		public Individual(SyntaxTree syntaxTree) {
			SyntaxTree = syntaxTree;
		}

		public Individual(Individual other) {
			SyntaxTree = (SyntaxTree) other.SyntaxTree.DeepClone();
			Fitness = other.Fitness;
			FitnessEvaluated = other.FitnessEvaluated;
		}

		public int CompareTo(Individual other) {
			return Fitness.CompareTo(other.Fitness);
		}

		public bool IsBetterThan(Individual other) {
			return Fitness < other.Fitness;
		}

		public override string ToString() {
			return SyntaxTree.ToString();
		}

		public override bool Equals(object obj) {
			return SyntaxTree.Equals(obj);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
