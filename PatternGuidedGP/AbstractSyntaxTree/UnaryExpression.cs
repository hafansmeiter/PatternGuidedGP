using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class UnaryExpression<TResult, TChild> : Expression<TResult> {
		public override Type[] ChildTypes => new[] { typeof(TChild) };

		public override IEnumerable<TreeItem> Children {
			get {
				yield return Child;
			}
		}

		public Expression<TChild> Child { get; set; }
	}
}
