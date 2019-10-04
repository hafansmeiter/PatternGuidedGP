using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using PatternGuidedGP.AbstractSyntaxTree.SyntaxGenerator.CSharp;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class TreeNode : ICSharpSyntaxGenerator, IChildAcceptor, ICloneable {
		public abstract bool IsTerminal{ get; }
		public abstract bool IsVariable { get; }
		public abstract int RequiredTreeDepth { get; }

		public abstract Type Type { get; }
		public abstract Type[] ChildTypes { get; }

		public List<TreeNode> Children { get; set; }

		// nodes get cloned -> no constructor 
		// use method Initialize instead
		public virtual void Initialize() {
		}

		protected abstract CSharpSyntaxNode GenerateSyntax();

		// interface ICloneable
		public object Clone() {
			TreeNode copy = (TreeNode) base.MemberwiseClone();
			copy.Children = new List<TreeNode>();
			return copy;
		}

		// interface ICSharpSyntaxGenerator
		public virtual CSharpSyntaxNode GetSyntaxNode() {
			return GenerateSyntax().WithAdditionalAnnotations(
				new SyntaxAnnotation("Node"),
				new SyntaxAnnotation("Type", Type.ToString()));
		}

		// interface IChildAcceptor
		public virtual bool AcceptChild(TreeNode child, int index) {
			return true;
		}
	}
}
