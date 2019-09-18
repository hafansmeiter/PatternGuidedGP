using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class UnaryExpression<TResult, TChild> : Expression<TResult> {
		public override Type[] ChildTypes => new[] { typeof(TChild) };
		public override bool IsTerminal => false;
		public override bool IsVariable => false;
		public override bool IsChildCountFixed => true;

		public Expression<TChild> Child {
			get {
				return Children[0] as Expression<TChild>;
			}
		}
	}
}
