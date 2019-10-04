using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Operators {
	abstract class InitializerBase : IInitializer {
		public TreeNodeRepository TreeNodeRepository { get; set; }

		public InitializerBase(TreeNodeRepository repository) {
			TreeNodeRepository = repository;
		}

		public abstract void Initialize(Population population, int maxTreeDepth, Type rootType);
	}
}
