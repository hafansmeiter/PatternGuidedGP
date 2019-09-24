using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP {
	class TestCase {
		public object[] Parameter { get; set; }
		public object Result { get; set; }

		public TestCase(object[] parameter, object result) {
			Parameter = parameter;
			Result = result;
		}

		public override string ToString() {
			StringBuilder builder = new StringBuilder();
			builder.Append("[Test case: ");
			foreach (var param in Parameter) {
				builder.Append(param.ToString() + " ");
			}
			builder.Append("; Result: " + Result.ToString());
			builder.Append("]");
			return builder.ToString();
		}
	}
}
