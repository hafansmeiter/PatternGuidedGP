using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Tests {
	class IntTestSuiteGenerator : ITestSuiteGenerator {
		public TestSuite Create(int parameterCount, Func<object[], object> resultDelegate) {
			var cases = new List<TestCase>();
			int count = (int)Math.Pow(parameterCount, parameterCount);
			for (int i = 0; i < count; i++) {
				object[] parameters = new object[parameterCount];
				for (int j = 0; j < parameterCount; j++) {
					int k = ((int)Math.Pow(parameterCount, parameterCount - j - 1));
					parameters[j] = (i / k) % parameterCount + 1;
				}
				var result = resultDelegate.Invoke(parameters);
				cases.Add(new TestCase(parameters, result));
			}
			return new TestSuite(cases);
		}
	}
}
