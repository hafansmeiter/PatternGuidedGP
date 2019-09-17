using Microsoft.CodeAnalysis.CSharp;
using PatternGuidedGP.AbstractSyntaxTree.SyntaxGenerator.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class Expression<TResult> : TypedTreeItem {
		public override Type Type => typeof(TResult);
		// default values for most expressions
		public override int Depth => 2;
		public override bool IsChildCountFixed => true;
	}
}
