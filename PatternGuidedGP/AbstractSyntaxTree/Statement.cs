using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class Statement : TypedTreeItem {
		public override Type Type => typeof(void);
		public override Type[] ChildTypes => new[] { typeof(void) };
		public override bool IsChildCountFixed => false;

		public override IEnumerable<TreeItem> Children {
			get {
				return _children;
			}
		}

		private IList<Statement> _children;
	}
}
