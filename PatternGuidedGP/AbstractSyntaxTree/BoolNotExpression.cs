using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class BoolNotExpression : UnaryExpression<bool, bool> {
		public override string Description => "!";

		public override SyntaxKind GetKind() {
			return SyntaxKind.LogicalNotExpression;
		}
	}
}
