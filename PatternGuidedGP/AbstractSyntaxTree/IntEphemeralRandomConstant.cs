using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using PatternGuidedGP.Util;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class IntEphemeralRandomConstant : EphemeralRandomConstant<int> {

		public IntEphemeralRandomConstant(int lowerBound, int upperBound) : base(lowerBound, upperBound) {
		}

		protected override CSharpSyntaxNode GenerateSyntax() {
			return SyntaxFactory.LiteralExpression(
				SyntaxKind.NumericLiteralExpression,
				SyntaxFactory.Literal((int)Value));
		}

		protected override int GetRandomValue() {
			return RandomValueGenerator.Instance.GetInt(LowerBound, UpperBound);
		}
	}
}
