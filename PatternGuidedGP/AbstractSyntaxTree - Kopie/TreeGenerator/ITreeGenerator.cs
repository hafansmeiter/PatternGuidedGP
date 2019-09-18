using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.TreeGenerator {
	interface ITreeGenerator {
		SyntaxTree GenerateTree(int maxDepth);
	}
}
