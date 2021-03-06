﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class IntMultiplicationExpression : BinaryExpression<int, int, int> {
		public override string Description => "*";
		public override int OperatorId => 205;

		public override SyntaxKind GetKind() {
			return SyntaxKind.MultiplyExpression;
		}

		public override bool IsInvertible => true;

		public override IEnumerable<object> Invert(object desiredValue, int k, object complementValue, out bool ambiguous) {
			var desired = (int)desiredValue;
			var complement = (int)complementValue;
			if (complement != 0) {
				ambiguous = false;
				return new object[] { desired / complement };
			}
			else {
				if (desired == 0) {
					ambiguous = true;
				} else {
					ambiguous = false;
				}
				return Enumerable.Empty<object>();
			}
		}
	}
}
