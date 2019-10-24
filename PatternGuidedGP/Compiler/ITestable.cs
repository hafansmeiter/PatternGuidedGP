using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Compiler.CSharp {
	interface ITestable {
		object RunTest(params object [] parameter);
	}
}
