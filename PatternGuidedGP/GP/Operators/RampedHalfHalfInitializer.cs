using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Operators {
	class RampedHalfHalfInitializer : InitializerBase {
		private KozaTreeGenerator _generatorFull = new KozaTreeGeneratorFull();
		private KozaTreeGenerator _generatorGrow = new KozaTreeGeneratorGrow();

		public RampedHalfHalfInitializer(int maxTreeDepth, IInstructionSetRepository repository) : base(maxTreeDepth, repository) {
		}

		public override void Initialize(Population population, Type rootType) {
			_generatorFull.InstructionSetRepository = InstructionSetRepository;
			_generatorGrow.InstructionSetRepository = InstructionSetRepository;
			for (int i = 0; !population.IsFull; i++) {
				var syntaxTree = new SyntaxTree();
				var rootStatements = RandomValueGenerator.Instance.GetInt(SyntaxConfiguration.Current.RootMaxStatements) + 1;
				for (int j = 0; j < rootStatements; j++) {
					TreeNode root;
					if (i % 2 == 0) {
						root = _generatorFull.GetSyntaxTree(MaxTreeDepth, rootType);
					} else {
						root = _generatorGrow.GetSyntaxTree(MaxTreeDepth, rootType);
					}
					syntaxTree.AddRootNode(root);
				}
				population.Add(new Individual(syntaxTree));
			}
		}
	}
}
