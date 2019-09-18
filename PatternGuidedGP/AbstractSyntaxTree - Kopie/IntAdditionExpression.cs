using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class IntAdditionExpression : BinaryExpression<int, int, int> {
		public override SyntaxKind GetKind() {
			return SyntaxKind.AddExpression;
		}
	}
}
