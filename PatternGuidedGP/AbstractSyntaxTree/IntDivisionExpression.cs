using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class IntDivisionExpression : BinaryExpression<int, int, int> {
		public override string Description => "/";

		public override SyntaxKind GetKind() {
			return SyntaxKind.DivideExpression;
		}

		public override bool IsInvertible => true;

		public override IEnumerable<object> Invert(object desiredValue, int k, object complementValue, out bool ambiguous) {
			var desired = (int)desiredValue;
			var complement = (int)complementValue;
			if (k == 0) {
				ambiguous = false;
				return new object[] { desired * complement };
			} else {
				if (complement != 0 && desired != 0) {
					ambiguous = false;
					return new object[] { complement / desired };
				} else if (desired == 0) {
					ambiguous = true;
				} else {
					ambiguous = false;
				}
				return Enumerable.Empty<object>();
			}
		}

		public override bool AcceptChild(TreeNode child, int index) {
			if (index != 1) {
				return true;
			}
			else {
				if (child is IntConstantExpression) {
					var constant = child as IntConstantExpression;
					return (int)constant.Value != 0;
				} else if (child is FloatConstantExpression) {
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
