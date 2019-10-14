using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.Pangea;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class AssignmentStatement<T> : Statement {
		public override string Description => "=";
		public override bool IsTerminal => false;
		public override bool IsVariable => false;
		public override int RequiredTreeDepth => 2;
		public override bool IsTraceable => true;
		public override Type[] ChildTypes => new[] { typeof(T), typeof(T) };

		public IdentifierExpression<T> Variable => Children[0] as IdentifierExpression<T>;
		public Expression<T> AssignedExpression => Children[1] as Expression<T>;

		protected override CSharpSyntaxNode GenerateSyntax() {
			return SyntaxFactory.ExpressionStatement(
					SyntaxFactory.AssignmentExpression(
						SyntaxKind.SimpleAssignmentExpression,
						(ExpressionSyntax)Variable.GetSyntaxNode(),
						(ExpressionSyntax)AssignedExpression.GetSyntaxNode()));
		}

		// accept only identifiers on left hand side
		public override bool AcceptChild(TreeNode child, int index) {
			if (index > 0) {
				return true;
			} else {
				if (child.IsVariable) {
					IdentifierExpression<T> variable = child as IdentifierExpression<T>;
					return variable.IsTargetVariable;
				} else {
					return false;
				}
			}
		}

		public override TreeNodeFilter GetChildSelectionFilter(int childIndex) {
			if (childIndex == 0) {
				return (nodes) => nodes.Where(node => AcceptChild(node, childIndex));
			} else {
				return base.GetChildSelectionFilter(childIndex);
			}
		}

		public override IEnumerable<TreeNode> GetExecutionTraceNodes() {
			return AssignedExpression.GetSubTreeNodes(true);
		}
	}
}
