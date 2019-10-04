using Microsoft.CodeAnalysis;
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

		public RampedHalfHalfInitializer(TreeNodeRepository repository) : base(repository) {
		}

		public override void Initialize(Population population, int maxTreeDepth, Type rootType) {
			_generatorFull.TreeNodeRepository = TreeNodeRepository;
			_generatorGrow.TreeNodeRepository = TreeNodeRepository;
			for (int i = 0; i < population.Size; i++) {
				SyntaxNode syntax;
				if (i % 2 == 0) {
					syntax = _generatorFull.GetSyntaxTree(maxTreeDepth, rootType);
				} else {
					syntax = _generatorGrow.GetSyntaxTree(maxTreeDepth, rootType);
				}
				Individual individual = new Individual(syntax);
				population.Add(individual);
			}
		}
	}
}
