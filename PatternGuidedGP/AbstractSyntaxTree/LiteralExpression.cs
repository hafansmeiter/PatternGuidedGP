using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class LiteralExpression<T> : NullaryExpression<T> {
		public override bool IsVariable => false;
		public override string Description => Value.ToString();
		public T Value { get; set; }

		protected LiteralExpression(T value) {
			Value = value;
		}
	}
}
