﻿using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class BoolLessThanIntExpression : BinaryExpression<bool, int, int> {
		public override string Description => "<";

		public override SyntaxKind GetKind() {
			return SyntaxKind.LessThanExpression;
		}

		public override bool IsInvertible => true;

		public override IEnumerable<object> Invert(object desiredValue, int k, object complementValue, out bool ambiguous) {
			ambiguous = true;
			return Enumerable.Empty<object>();
		}
	}
}
