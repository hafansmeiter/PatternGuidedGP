using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.Pool;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP;
using PatternGuidedGP.GP.Operators;
using PatternGuidedGP.GP.SemanticGP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.SemanticGP {
	class ApproximatelySemanticMutator : IMutator, ISemanticOperator {
		public IResultSemanticsOperator ResultSemanticsOperator { get; set; } = new PawlakRandomDesiredOperator();
		public ISemanticSubTreePool SubTreePool { get; set; }
		public Semantics DesiredSemantics { get; set; }
		public int MaxTreeDepth { get; set; }

		public ApproximatelySemanticMutator(ISemanticSubTreePool subTreePool, int maxTreeDepth) {
			SubTreePool = subTreePool;
			MaxTreeDepth = MaxTreeDepth;
		}
		
		public bool Mutate(Individual individual) {
			return ResultSemanticsOperator.Operate(DesiredSemantics, individual, SubTreePool, MaxTreeDepth);
		}
	}
}
