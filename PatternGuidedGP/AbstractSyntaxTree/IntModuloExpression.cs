using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class IntModuloExpression : BinaryExpression<int, int, int> {
		public override string Description => "%";
		public override int OperatorId => 204;

		public override SyntaxKind GetKind() {
			return SyntaxKind.ModuloExpression;
		}

		public override bool AcceptChild(TreeNode child, int index) {
			bool accept = true;
			if (index == 1) {
				if (child is IntLiteralExpression) {
					var constant = child as IntLiteralExpression;
					accept = (int)constant.Value != 0;
				}
				else if (child is FloatLiteralExpression) {
					var constant = child as FloatLiteralExpression;
					accept = (float)constant.Value != 0;
				}
			}
			return accept ? base.AcceptChild(child, index) : false;
		}

		public override TreeNodeFilter GetChildSelectionFilter(int childIndex) {
			if (childIndex == 1) {
				return (nodes) => nodes.Where(node => AcceptChild(node, childIndex));
			}
			else {
				return base.GetChildSelectionFilter(childIndex);
			}
		}
	}
}
