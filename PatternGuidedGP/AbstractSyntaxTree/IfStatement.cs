using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.Pangea;
using PatternGuidedGP.Util;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class IfStatement : Statement, ITraceable {
		public override string Description => "if";
		public override int RequiredTreeDepth => 3;
		public override Type[] ChildTypes => _childTypes.ToArray();
		public bool IsTraceable => true;
		public override int OperatorId => (int) Operators.IfStatement;

		public bool HasElseClause { get; set; } = true;

		public Expression<bool> Condition => Children[0] as Expression<bool>;
		public Statement[] IfBlockStatements => Children.GetRange(1, _ifStatements).Select(c => (Statement) c).ToArray();
		public Statement[] ElseBlockStatements => Children.GetRange(_ifStatements + 1, _elseStatements).Select(c => (Statement) c).ToArray();

		private IList<Type> _childTypes = new List<Type>();

		private int _ifStatements = SyntaxConfiguration.Current.MaxIfBlockStatements;
		private int _elseStatements = SyntaxConfiguration.Current.MaxElseBlockStatement;

		public override void Initialize() {
			base.Initialize();
			_childTypes = new List<Type>(new[] { typeof(bool) });
			// create if block
			CreateBlock(SyntaxConfiguration.Current.MaxIfBlockStatements, ref _ifStatements);

			// randomly choose if else-clause is present
			if (RandomValueGenerator.Instance.GetDouble() < SyntaxConfiguration.Current.IfHasElseBlockPropability) {
				HasElseClause = true;
				CreateBlock(SyntaxConfiguration.Current.MaxElseBlockStatement, ref _elseStatements);
			} else {
				HasElseClause = false;
			}
		}

		private void CreateBlock(int maxStatements, ref int actualStatements) {
			actualStatements = RandomValueGenerator.Instance.GetInt(maxStatements) + 1;
			for (int i = 0; i < actualStatements; i++) {
				_childTypes.Add(typeof(void));
			}
		}

		protected override CSharpSyntaxNode GenerateSyntax() {
			if (HasElseClause) {
				return SyntaxFactory.IfStatement((ExpressionSyntax) Condition.GetSyntaxNode(), 
					SyntaxFactory.Block(IfBlockStatements.Select(s => (StatementSyntax) s.GetSyntaxNode())))
				.WithElse(SyntaxFactory.ElseClause(SyntaxFactory.Block(ElseBlockStatements.Select(s => (StatementSyntax) s.GetSyntaxNode()))));
			} else {
				return SyntaxFactory.IfStatement((ExpressionSyntax)Condition.GetSyntaxNode(),
					SyntaxFactory.Block(IfBlockStatements.Select(s => (StatementSyntax)s.GetSyntaxNode())));
			}
		}

		public override CSharpSyntaxNode GetExecutionStateSyntaxNode() {
			return Condition.GetSyntaxNode();
		}

		public IEnumerable<TreeNode> GetExecutionTraceNodes() {
			return new[] { this }.Concat(Condition.GetSubTreeNodes(true));
		}
	}
}
