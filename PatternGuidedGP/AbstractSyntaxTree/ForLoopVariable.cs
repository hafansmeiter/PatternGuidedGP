﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class ForLoopVariable : IntIdentifierExpression, IScoped {
		public bool IsScoped => true;

		// name is assigned dynamically before compiling the syntax
		// name will be the loop variable name (e.g. i4) of the enclosing for-loop
		public ForLoopVariable(bool assignable = false) : base("i", assignable) {
		}

		public bool IsInScopeOf(TreeNode node) {
			return node is ForLoopTimesStatement;
		}
	}
}
