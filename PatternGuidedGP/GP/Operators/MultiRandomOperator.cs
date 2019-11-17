using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Operators {
	class MultiRandomOperator<TOperator> {
		public IList<TOperator> Options { get; set; } = new List<TOperator>();

		protected TOperator GetRandom() {
			return Options[RandomValueGenerator.Instance.GetInt(Options.Count)];
		}
	}
}
