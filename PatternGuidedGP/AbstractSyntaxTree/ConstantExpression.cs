using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class ConstantExpression<T> : NullaryExpression<T> {
		public override bool IsVariable => false;
		public override string Description => Value.ToString();
		public object Value { get; set; }

		protected ConstantExpression(object value) {
			Value = value;
		}
	}
}
