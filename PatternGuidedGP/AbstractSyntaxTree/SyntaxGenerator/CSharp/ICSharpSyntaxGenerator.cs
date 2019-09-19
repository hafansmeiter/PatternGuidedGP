using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.SyntaxGenerator.CSharp {
	interface ICSharpSyntaxGenerator {
		CSharpSyntaxNode GetSyntaxNode();
	}
}
