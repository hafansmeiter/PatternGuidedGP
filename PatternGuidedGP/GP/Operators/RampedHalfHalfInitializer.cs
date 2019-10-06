using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Operators {
	class RampedHalfHalfInitializer : InitializerBase {
		private KozaTreeGenerator _generatorFull = new KozaTreeGeneratorFull();
		private KozaTreeGenerator _generatorGrow = new KozaTreeGeneratorGrow();

		public RampedHalfHalfInitializer(int maxTreeDepth, TreeNodeRepository repository) : base(maxTreeDepth, repository) {
		}

		public override void Initialize(Population population, Type rootType) {
			_generatorFull.TreeNodeRepository = TreeNodeRepository;
			_generatorGrow.TreeNodeRepository = TreeNodeRepository;
			for (int i = 0; i < population.Size; i++) {
				SyntaxTree syntaxTree;
				if (i % 2 == 0) {
					syntaxTree = _generatorFull.GetSyntaxTree(MaxTreeDepth, rootType);
				} else {
					syntaxTree = _generatorGrow.GetSyntaxTree(MaxTreeDepth, rootType);
				}
				Individual individual = new Individual(syntaxTree);
				population.Add(individual);
			}
		}
	}
}
