using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class NullaryExpression<TResult> : Expression<TResult> {
		public override Type[] ChildTypes => new Type[] { };
		public override bool IsTerminal => true;
		public override int RequiredTreeDepth => 1;
	}
}
