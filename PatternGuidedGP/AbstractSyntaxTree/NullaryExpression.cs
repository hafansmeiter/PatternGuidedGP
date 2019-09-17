using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class NullaryExpression<TResult> : Expression<TResult> {
		public override Type[] ChildTypes => new Type[] { };
		public override int Depth => 0;

		public override IEnumerable<TreeItem> Children {
			get {
				return Enumerable.Empty<TreeItem>();
			}
		}
	}
}
