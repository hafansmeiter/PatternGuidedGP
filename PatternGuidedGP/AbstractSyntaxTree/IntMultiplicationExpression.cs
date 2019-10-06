﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class IntMultiplicationExpression : BinaryExpression<int, int, int> {
		public override string Description => "*";

		public override SyntaxKind GetKind() {
			return SyntaxKind.MultiplyExpression;
		}
	}
}
