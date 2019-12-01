using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class IntModuloExpression : BinaryExpression<int, int, int> {
		public override string Description => "%";

		public override SyntaxKind GetKind() {
			return SyntaxKind.ModuloExpression;
		}

		public override bool AcceptChild(TreeNode child, int index) {
			if (index != 1) {
				return true;
			}
			else {
				if (child is IntConstantExpression) {
					var constant = child as IntConstantExpression;
					return (int)constant.Value != 0;
				}
				else if (child is FloatConstantExpression) {
					var constant = child as FloatConstantExpression;
					return (float)constant.Value != 0;
				}
				else {
					return true;
				}
			}
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
