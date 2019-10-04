using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.Util;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class BlockStatement : Statement {
		public override bool IsTerminal => false;
		public override bool IsVariable => false;
		public override int RequiredTreeDepth => 0;
		public override Type[] ChildTypes => _childTypes;
		public override bool IsContainer => true;

		public int MaxChildCount { get; set; } = 4;
	
		private Type[] _childTypes;

		public override void Initialize() {
			int childCount = RandomValueStore.Instance.GetInt(MaxChildCount) + 1;
			_childTypes = new Type[childCount];
			for (int i = 0; i < childCount; i++) {
				_childTypes[i] = typeof(void);
			}
		}

		public override CSharpSyntaxNode GetSyntaxNode() {
			// do not annotate as Node, as BlockStatement is a container.
			return GenerateSyntax();
		}

		protected override CSharpSyntaxNode GenerateSyntax() {
			var statementSyntaxList = new List<StatementSyntax>();
			foreach (var statement in Children) {
				GetContentSyntax((Statement) statement, statementSyntaxList);
			}
			return SyntaxFactory.Block(statementSyntaxList);
		}

		private void GetContentSyntax(Statement statement, IList<StatementSyntax> syntaxList) {
			if (statement.IsContainer) {
				foreach (var child in statement.Children) {
					GetContentSyntax((Statement) child, syntaxList);
				}
			} else {
				syntaxList.Add(statement.GetSyntaxNode() as StatementSyntax);
			}
		}
	}
}
