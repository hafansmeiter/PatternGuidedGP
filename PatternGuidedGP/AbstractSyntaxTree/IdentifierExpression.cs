using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class IdentifierExpression<T> : NullaryExpression<T> {
		public override string Description => Name;
		public override bool IsVariable => true;
		public string Name { get; set; }

		public IdentifierExpression(string name) {
			Name = name;
		}

		protected override CSharpSyntaxNode GenerateSyntax() {
			return SyntaxFactory.IdentifierName(Name);
		}
	}
}
