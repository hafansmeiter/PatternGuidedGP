using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class SyntaxConfiguration {
		public static SyntaxConfiguration Current { get; set; } = new Simple();

		private SyntaxConfiguration() {
		}

		// Root syntax
		public int RootMaxStatements { get; set; } = 3;

		// For loop
		public int ForLoopMaxIterations { get; set; } = 10000;
		public int ForLoopMaxStatements { get; set; } = 3;

		// If statement
		public double IfHasElseBlockPropability { get; set; } = 0.5;
		public int MaxIfBlockStatements { get; set; } = 3;
		public int MaxElseBlockStatement { get; set; } = 3;

		public class Simple : SyntaxConfiguration {
			public Simple() {
				RootMaxStatements = 1;
				ForLoopMaxStatements = 1;
				ForLoopMaxIterations = 10000;
				IfHasElseBlockPropability = 1;
				MaxIfBlockStatements = 1;
				MaxElseBlockStatement = 1;
			}
		}

		public class Advanced : SyntaxConfiguration {
			public Advanced() {
				RootMaxStatements = 3;
				ForLoopMaxStatements = 3;
				ForLoopMaxIterations = 10000;
				IfHasElseBlockPropability = 0.5;
				MaxIfBlockStatements = 3;
				MaxElseBlockStatement = 3;
			}
		}
	}
}
