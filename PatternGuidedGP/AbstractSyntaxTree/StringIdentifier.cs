﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class StringIdentifierExpression : IdentifierExpression<string> {
		public StringIdentifierExpression(string name, bool assignable = false) : base(name, assignable) {
		}
	}
}
