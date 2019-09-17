using Microsoft.CodeAnalysis.CSharp;
using PatternGuidedGP.AbstractSyntaxTree.SyntaxGenerator.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class TreeItem : ICSharpSyntaxGenerator {
		public TreeItem Parent { get; protected set; }
		public abstract IEnumerable<TreeItem> Children { get; }
		public abstract int Depth { get; }
		public abstract bool IsChildCountFixed { get; }

		public abstract CSharpSyntaxNode GenerateSyntax();
	}
}
