using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class AssignmentStatement<T> : Statement {
		public override string Description => "=";
		public override bool IsTerminal => false;
		public override bool IsVariable => false;
		public override int RequiredTreeDepth => 2;
		public override bool IsContainer => false;
		public override Type[] ChildTypes => new[] { typeof(T), typeof(T) };

		public IdentifierExpression<T> Variable => Children[0] as IdentifierExpression<T>;
		public Expression<T> Expression => Children[1] as Expression<T>;

		protected override CSharpSyntaxNode GenerateSyntax() {
			return SyntaxFactory.ExpressionStatement(
				SyntaxFactory.AssignmentExpression(
					SyntaxKind.SimpleAssignmentExpression,
					((ExpressionSyntax) Variable.GetSyntaxNode()).WithoutAnnotations("Node"),
					(ExpressionSyntax) Expression.GetSyntaxNode()));
		}

		// accept only identifiers on left hand side
		public override bool AcceptChild(TreeNode child, int index) {
			if (index > 0) {
				return true;
			} else {
				return child.IsVariable;
			}
		}

		public override TreeNodeFilter GetChildSelectionFilter(int childIndex) {
			if (childIndex == 0) {
				return (nodes) => nodes.Where(node => node.IsVariable);
			} else {
				return base.GetChildSelectionFilter(childIndex);
			}
		}
	}
}
