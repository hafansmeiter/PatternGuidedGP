using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class BoolIdentifierExpression : IdentifierExpression<bool> {
		public BoolIdentifierExpression(string name, bool assignable = false) : base(name, assignable) {
		}
	}
}
