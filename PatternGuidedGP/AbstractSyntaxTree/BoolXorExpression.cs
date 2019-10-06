using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class BoolXorExpression : BinaryExpression<bool, bool, bool> {
		public override string Description => "XOR";

		public override SyntaxKind GetKind() {
			return SyntaxKind.ExclusiveOrExpression;
		}
	}
}
