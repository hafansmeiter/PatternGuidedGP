using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.Pool {
	class RandomPoolItemSelector<T> : IPoolItemSelector<T> {
		public T DrawFromList(IList<T> list) {
			return list[RandomValueGenerator.Instance.GetInt(list.Count)];
		}
	}
}
