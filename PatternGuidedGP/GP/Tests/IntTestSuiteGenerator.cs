using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Tests {
	class IntTestSuiteGenerator : ITestSuiteGenerator {
		public TestSuite Create(int parameterCount, Func<object[], object> resultDelegate) {
			return new TestSuite(null);
		}
	}
}
