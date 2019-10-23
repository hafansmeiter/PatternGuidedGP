using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.Pool {
	public interface IPoolItemSelector<T> {
		T DrawFromList(IList<T> list);
	}
}
