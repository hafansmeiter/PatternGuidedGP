using Accord.Collections;
using PatternGuidedGP.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Pangea {
	interface ITraceable {
		bool IsTraceable { get; }
		IEnumerable<TreeNode> GetExecutionTraceNodes();
	}
}
