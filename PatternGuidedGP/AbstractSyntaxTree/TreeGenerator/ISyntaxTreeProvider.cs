﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.TreeGenerator {
	interface ISyntaxTreeProvider {
		TreeNode GetSyntaxTree(int maxDepth, Type type);
	}
}
