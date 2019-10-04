using Microsoft.CodeAnalysis;
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
		public SyntaxNode Syntax { get; set; }

		private double _fitness = 0.0;
		private bool _fitnessEvaluated = false;

		public Individual(SyntaxNode syntax) {
			Syntax = syntax;
		}

		public Individual(Individual other) {
			Syntax = other.Syntax;
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
			return Syntax.NormalizeWhitespace().ToString();
		}

		public override bool Equals(object obj) {
			return Syntax.Equals(obj);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
