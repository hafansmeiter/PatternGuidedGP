﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class IntLiteralExpression : LiteralExpression<int> {

		public IntLiteralExpression(int value) : base(value) {
		}

		protected override CSharpSyntaxNode GenerateSyntax() {
			return SyntaxFactory.LiteralExpression(
				SyntaxKind.NumericLiteralExpression,
				SyntaxFactory.Literal((int) Value));
		}
	}
}
