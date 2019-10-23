using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.GP.Tests;

namespace PatternGuidedGP.GP.Problems {
	class MultiplexerProblem : CodingProblem {
		public override Type ReturnType => typeof(bool);
		public override Type ParameterType => typeof(bool);

		private int _addressBits;

		public MultiplexerProblem(int n, int addressBits) : base(n, false) {
			_addressBits = addressBits;
			Initialize();
		}

		protected override TestSuite GetTestSuite() {
			return new BoolTestSuiteGenerator().Create(ParameterCount, parameters => {
				int address = 0;
				for (int i = 0; i < _addressBits; i++) {
					address <<= 1;
					address += ((bool)parameters[i]) ? 1 : 0;
				}
				return parameters[address + _addressBits];
			});
		}
	}
}
