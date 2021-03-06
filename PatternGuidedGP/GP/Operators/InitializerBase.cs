﻿using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Operators {
	abstract class InitializerBase : IInitializer {
		public IInstructionSetRepository InstructionSetRepository { get; set; }
		public int MaxTreeDepth { get; set; }

		public InitializerBase(int maxTreeDepth, IInstructionSetRepository repository) {
			InstructionSetRepository = repository;
			MaxTreeDepth = maxTreeDepth;
		}

		public abstract void Initialize(Population population, Type rootType);
	}
}
