using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class TypedTreeItem : TreeItem {
		public abstract Type Type { get; }
		public abstract Type[] ChildTypes { get; }
	}
}
