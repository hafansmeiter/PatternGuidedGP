using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Tests {
	class BoolTestSuiteGenerator : ITestSuiteGenerator {
		public TestSuite Create(int parameterCount, Func<object[], object> resultDelegate) {
			IList<TestCase> cases = new List<TestCase>();
			int count = (int)Math.Pow(2, parameterCount);
			for (int i = 0; i < count; i++) {
				var parameters = Convert.ToString(i, 2)
					.PadLeft(parameterCount, '0')
					.ToCharArray().Select(c => (object) (c == '1')).ToArray();
				var result = resultDelegate.Invoke(parameters);
				cases.Add(new TestCase(parameters, result));
			}
			return new TestSuite(cases);
		}
	}
}
