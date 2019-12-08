using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	// Ephemeral random constants (ERCs) described in:
	// Helmuth, Spector - Detailed Problem Descriptions for 
	//   General Program Synthesis Benchmark Suite (2015) (page 9)
	abstract class EphemeralRandomConstant<T> : NullaryExpression<T> {
		public override bool IsVariable => false;
		public override string Description => Value != null ? Value.ToString() : "";
		public T Value { get; set; }

		public T LowerBound { get; private set; }
		public T UpperBound { get; private set; }

		protected EphemeralRandomConstant(T lowerBound, T upperBound) {
			LowerBound = lowerBound;
			UpperBound = upperBound;
		}

		public override void Initialize() {
			Value = GetRandomValue();
		}

		protected abstract T GetRandomValue();
	}
}
