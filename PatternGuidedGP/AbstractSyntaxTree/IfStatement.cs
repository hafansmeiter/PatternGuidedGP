using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.Util;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class IfStatement : Statement {
		public override string Description => "if";
		public override bool IsTerminal => false;
		public override bool IsVariable => false;
		public override int RequiredTreeDepth => 3;
		public override Type[] ChildTypes {
			get => _childTypes;
		}
		public bool HasElseClause { get; private set; }

		public Expression<bool> Condition => Children[0] as Expression<bool>;
		public Statement IfBlock => Children[1] as Statement;
		public Statement ElseBlock => Children[2] as Statement;

		private Type[] _childTypes;

		public override void Initialize() {
			// randomly choose if else-clause is present
			base.Initialize();
			if (RandomValueStore.Instance.GetInt(2) == 0) {
				HasElseClause = false;
				_childTypes = new[] { typeof(bool), typeof(void) };
			} else {
				HasElseClause = true;
				_childTypes = new[] { typeof(bool), typeof(void), typeof(void) };
			}
		}

		protected override CSharpSyntaxNode GenerateSyntax() {
			if (HasElseClause) {
				return SyntaxFactory.IfStatement((ExpressionSyntax) Condition.GetSyntaxNode(), 
					SyntaxFactory.Block((StatementSyntax) IfBlock.GetSyntaxNode()))
				.WithElse(SyntaxFactory.ElseClause(SyntaxFactory.Block((StatementSyntax) ElseBlock.GetSyntaxNode())));
			} else {
				return SyntaxFactory.IfStatement((ExpressionSyntax)Condition.GetSyntaxNode(),
					SyntaxFactory.Block((StatementSyntax)IfBlock.GetSyntaxNode()));
			}
		}
	}
}
