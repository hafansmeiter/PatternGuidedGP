using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.Pool {
	class RankBasedPoolItemSelector<T> : IPoolItemSelector<T> {

		public T DrawFromList(IList<T> list) {
			if (list.Count == 1) {
				return list[0];
			}
			int fitnessRanks = (1 + list.Count) * list.Count / 2;
			int rankNumber = RandomValueGenerator.Instance.GetInt(fitnessRanks);
			int rankIndex = 0;
			int aggregatedRanks = list.Count;
			while (aggregatedRanks <= rankNumber) {
				rankIndex++;
				aggregatedRanks += aggregatedRanks - 1;
			}
			return list[rankIndex];
		}
	}
}
