using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using PatternGuidedGP.AbstractSyntaxTree.SyntaxGenerator.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class TreeNode : ICSharpSyntaxGenerator, ICloneable {
		public List<TreeNode> Children { get; set; }
		public abstract bool IsTerminal{ get; }
		public abstract bool IsVariable { get; }
		public abstract bool IsChildCountFixed { get; }

		public abstract Type Type { get; }
		public abstract Type[] ChildTypes { get; }

		public object Clone() {
			TreeNode copy = (TreeNode) base.MemberwiseClone();
			copy.Children = new List<TreeNode>();
			return copy;
		}

		public CSharpSyntaxNode GetSyntaxNode() {
			return GenerateSyntax().WithAdditionalAnnotations(
				new SyntaxAnnotation("Node"),
				new SyntaxAnnotation("Type", Type.ToString()));
		}

		protected abstract CSharpSyntaxNode GenerateSyntax();
	}
}
