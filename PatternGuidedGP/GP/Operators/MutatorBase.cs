using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Operators {
	abstract class MutatorBase : IMutator {
		public ISyntaxProvider SyntaxTreeProvider { get; }
		public int MaxMutationTreeDepth { get; set; }
		public int MaxTreeDepth { get; set; }

		public MutatorBase(ISyntaxProvider provider, int maxTreeDepth, int maxMutationTreeDepth) {
			SyntaxTreeProvider = provider;
			MaxTreeDepth = maxTreeDepth;
			MaxMutationTreeDepth = maxMutationTreeDepth;
		}

		public abstract bool Mutate(Individual individual);
	}
}
