using PatternGuidedGP.GP.SemanticGP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.Pool {
	interface ISubTreePool {
		bool Add(TreeNode node, object data);
		TreeNode GetRandom(Type type);
		void RemoveWorstItems();
		void Clear();
		void TrimToMaxSize();
	}
}
