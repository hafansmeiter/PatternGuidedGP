﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.TreeGenerator {
	interface ITreeNodeRepository {
		void Add(params TreeNode[] treeNodes);
		TreeNode GetRandomAny(Type type, int maxDepth, TreeNodeFilter filter);
		TreeNode GetRandomNonTerminal(Type type, int maxDepth);
		TreeNode GetRandomTerminal(Type type);
	}
}
