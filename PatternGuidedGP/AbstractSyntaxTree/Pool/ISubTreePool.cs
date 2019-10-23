﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.Pool {
	interface ISubTreePool {
		bool Add(TreeNode node, double fitness);
		TreeNode GetRandom(Type type, int maxDepth);
	}
}
