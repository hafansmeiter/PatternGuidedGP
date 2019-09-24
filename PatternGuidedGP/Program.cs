using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.Compiler.CSharp;
using PatternGuidedGP.GP;
using PatternGuidedGP.GP.Operators;

namespace PatternGuidedGP {
	class Program {
		static void Main(string[] args) {
	
			CompilationUnitSyntax template = SyntaxFactory.CompilationUnit()
				.WithUsings(
					SyntaxFactory.SingletonList<UsingDirectiveSyntax>(
						SyntaxFactory.UsingDirective(
							SyntaxFactory.IdentifierName("System"))))
				.WithMembers(
					SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
						SyntaxFactory.ClassDeclaration("ProblemClass")
						.WithMembers(
							SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
								SyntaxFactory.MethodDeclaration(
									SyntaxFactory.PredefinedType(
										SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
									SyntaxFactory.Identifier("Test"))
								.WithAdditionalAnnotations(new SyntaxAnnotation("MethodDeclaration"))
								.WithModifiers(
									SyntaxFactory.TokenList(
										new[]{
											SyntaxFactory.Token(SyntaxKind.PublicKeyword),
											SyntaxFactory.Token(SyntaxKind.StaticKeyword)}))
								.WithParameterList(
									SyntaxFactory.ParameterList(
										SyntaxFactory.SeparatedList<ParameterSyntax>(
											new SyntaxNodeOrToken[] {
												SyntaxFactory.Parameter(
													SyntaxFactory.Identifier("a"))
												.WithType(
													SyntaxFactory.PredefinedType(
														SyntaxFactory.Token(SyntaxKind.BoolKeyword))),
												SyntaxFactory.Token(SyntaxKind.CommaToken),
												SyntaxFactory.Parameter(
													SyntaxFactory.Identifier("b"))
												.WithType(
													SyntaxFactory.PredefinedType(
														SyntaxFactory.Token(SyntaxKind.BoolKeyword))),
												SyntaxFactory.Token(SyntaxKind.CommaToken),
												SyntaxFactory.Parameter(
													SyntaxFactory.Identifier("c"))
												.WithType(
													SyntaxFactory.PredefinedType(
														SyntaxFactory.Token(SyntaxKind.BoolKeyword)))
											})))
									.WithAdditionalAnnotations(new SyntaxAnnotation("ParameterList"))
								.WithBody(
									SyntaxFactory.Block(
											SyntaxFactory.ReturnStatement(
												SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)
												.WithAdditionalAnnotations(new SyntaxAnnotation("ReturnValue"))
											)
										.WithAdditionalAnnotations(new SyntaxAnnotation("MethodBlock"))
									)
								)
							))))
				.NormalizeWhitespace();

			//Console.WriteLine(template.ToString());

			var generator = new KozaTreeGeneratorGrow();
			var repository = new TreeNodeRepository();
			repository.Add(new BoolAndExpression(),
				new BoolFalseExpression(),
				new BoolNotExpression(),
				new BoolOrExpression(),
				new BoolTrueExpression(),
				new BoolXorExpression(),
				new BoolEqualBoolExpression(),
				new BoolNotEqualBoolExpression(),
				new BoolIdentifierExpression("a"),
				new BoolIdentifierExpression("b"),
				new BoolIdentifierExpression("c"));
			generator.TreeNodeRepository = repository;

			var compiler = new CSharpCompiler();
			var evaluator = new ProgramFitnessEvaluator();
			evaluator.Compiler = compiler;
			evaluator.Template = template;

			TestSuite suite = new TestSuite();
			suite.TestCases.Add(new TestCase(new object[] { true, true, true }, true));
			suite.TestCases.Add(new TestCase(new object[] { true, true, false }, true));
			suite.TestCases.Add(new TestCase(new object[] { true, false, true }, true));
			suite.TestCases.Add(new TestCase(new object[] { true, false, false }, false));
			suite.TestCases.Add(new TestCase(new object[] { false, true, true }, true));
			suite.TestCases.Add(new TestCase(new object[] { false, true, false }, false));
			suite.TestCases.Add(new TestCase(new object[] { false, false, true }, false));
			suite.TestCases.Add(new TestCase(new object[] { false, false, false }, false));

			Problem problem = new Problem();
			problem.FitnessEvaluator = evaluator;
			problem.TestSuite = suite;

			DefaultAlgorithm algorithm = new DefaultAlgorithm(problem, 
				populationSize: 100, generations: 20);
			algorithm.Crossover = new RandomSubtreeCrossover(maxTreeDepth: 5);
			algorithm.CrossoverRate = 0.8;
			algorithm.Elitism = 5;
			algorithm.Initializer = new RampedHalfHalfInitializer(repository);
			algorithm.MaxTreeDepth = 5;
			algorithm.MutationRate = 0.1;
			algorithm.Mutator = new RandomSubtreeMutator(generator, maxTreeDepth: 5, maxMutationTreeDepth: 3);
			algorithm.Selector = new TournamentSelector(7);

			Individual individual = algorithm.Run();
			Console.WriteLine("Result solution: " + individual);

			Console.ReadKey();
		}
	}
}
