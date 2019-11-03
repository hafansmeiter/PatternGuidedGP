using PatternGuidedGP.GP.SemanticGP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Tests {
	class TestSuite {
		public IList<TestCase> TestCases { get; set; } = new List<TestCase>();
		public Semantics Semantics {
			get {
				if (_semantics == null) {
					_semantics = new Semantics(TestCases.Count);
					for (int i = 0; i < TestCases.Count; i++) {
						_semantics[i] = TestCases[i].Result;
					}
				}
				return _semantics;
			}
		}
		private Semantics _semantics;

		public TestSuite(IList<TestCase> cases) {
			TestCases = cases;
		}
	}
}
