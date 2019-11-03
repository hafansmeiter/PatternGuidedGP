using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.GP.SemanticGP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP {
	class Individual : IComparable<Individual>, ISemanticsProvider {

		private double _fitness = 0.0;
		public double Fitness {
			get {
				return _fitness;
			}
			set {
				_fitness = value;
				_fitnessEvaluated = true;
			}
		}

		private bool _fitnessEvaluated = false;
		public bool FitnessEvaluated {
			get {
				return _fitnessEvaluated;
			}
			set {
				_fitnessEvaluated = value;
			}
		}
		public SyntaxTree SyntaxTree { get; set; }

		private Semantics _semantics;
		public Semantics Semantics {
			get => _semantics;
			set => _semantics = value;
		}

		private bool _isSemanticsEvaluated;
		public bool IsSemanticsEvaluated {
			get => _isSemanticsEvaluated;
			set => _isSemanticsEvaluated = value;
		}


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
			var other = obj as Individual;
			return other != null && SyntaxTree.Equals(other.SyntaxTree);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
