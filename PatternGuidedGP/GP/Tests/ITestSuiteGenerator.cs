using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Tests {
	interface ITestSuiteGenerator {
		TestSuite Create(int parameterCount, Func<object[], object> resultDelegate);
	}
}
