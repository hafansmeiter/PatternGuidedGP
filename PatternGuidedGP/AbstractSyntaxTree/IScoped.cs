using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	interface IScoped {
		bool IsScoped { get; }

		bool IsInScopeOf(TreeNode node);
	}
}
