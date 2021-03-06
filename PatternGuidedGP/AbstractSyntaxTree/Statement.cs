﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class Statement : TreeNode {
		public override Type Type => typeof(void);
		public override bool IsTerminal => false;
		public override bool IsVariable => false;
	}
}
