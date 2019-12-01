using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.Pangea;
using PatternGuidedGP.Util;

namespace PatternGuidedGP.AbstractSyntaxTree {
	class ForLoopTimesStatement : Statement, ITraceable {
		public override string Description => "for";
		public override bool IsTerminal => false;
		public override bool IsVariable => false;
		public override int RequiredTreeDepth => 3;
		public override Type[] ChildTypes => _childTypes.ToArray();
		public bool IsTraceable => true;
		public override int OperatorId => 001;

		public Expression<int> Count => Children[0] as Expression<int>;
		public Statement[] ContentStatements => Children.GetRange(1, _childTypes.Count - 1).Select(c => (Statement)c).ToArray();

		private IList<Type> _childTypes = new List<Type>();

		public string LoopVariableName => "i" + Id;

		public override void Initialize() {
			base.Initialize();
			_childTypes = new List<Type>(new[] { typeof(int) });
			var statements = RandomValueGenerator.Instance.GetInt(SyntaxConfiguration.Current.ForLoopMaxStatements) + 1;
			for (int i = 0; i < statements; i++) {
				_childTypes.Add(typeof(void));
			}
		}

		public IEnumerable<TreeNode> GetExecutionTraceNodes() {
			return new[] { Count };
		}

		protected override CSharpSyntaxNode GenerateSyntax() {
			string loopVariableName = LoopVariableName;
			return SyntaxFactory.ForStatement(
						SyntaxFactory.Block(ContentStatements.Select(s => (StatementSyntax)s.GetSyntaxNode())))
					.WithDeclaration(
						SyntaxFactory.VariableDeclaration(
							SyntaxFactory.PredefinedType(
								SyntaxFactory.Token(SyntaxKind.IntKeyword)))
						.WithVariables(
							SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
								SyntaxFactory.VariableDeclarator(
									SyntaxFactory.Identifier(loopVariableName))
								.WithInitializer(
									SyntaxFactory.EqualsValueClause(
										SyntaxFactory.LiteralExpression(
											SyntaxKind.NumericLiteralExpression,
											SyntaxFactory.Literal(0)))))))
					.WithCondition(
						SyntaxFactory.BinaryExpression(
							SyntaxKind.LessThanExpression,
							SyntaxFactory.IdentifierName(loopVariableName),
							SyntaxFactory.InvocationExpression(
								SyntaxFactory.MemberAccessExpression(
									SyntaxKind.SimpleMemberAccessExpression,
									SyntaxFactory.IdentifierName("Math"),
									SyntaxFactory.IdentifierName("Min")))
							.WithArgumentList(
								SyntaxFactory.ArgumentList(
									SyntaxFactory.SeparatedList<ArgumentSyntax>(
										new SyntaxNodeOrToken[]{
											SyntaxFactory.Argument(
												SyntaxFactory.LiteralExpression(
													SyntaxKind.NumericLiteralExpression,
													SyntaxFactory.Literal(SyntaxConfiguration.Current.ForLoopMaxIterations))),
											SyntaxFactory.Token(SyntaxKind.CommaToken),
											SyntaxFactory.Argument(
												(ExpressionSyntax) Count.GetSyntaxNode())})))))
					.WithIncrementors(
						SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
							SyntaxFactory.PostfixUnaryExpression(
								SyntaxKind.PostIncrementExpression,
								SyntaxFactory.IdentifierName(loopVariableName))));
		}
	}
}
