using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Tests {
	class TestSuite {
		public IList<TestCase> TestCases { get; set; } = new List<TestCase>();

		public TestSuite(IList<TestCase> cases) {
			TestCases = cases;
		}
	}
}
