using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.TreeGenerator {
	interface ISyntaxTreeProvider {
		SyntaxNode GetSyntaxTree(int maxDepth);
		SyntaxNode GetTypedSyntaxTree(int maxDepth, Type type);
	}
}
