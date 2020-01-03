using PatternGuidedGP.GP.Problems;
using PatternGuidedGP.GP.Problems.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PatternGuidedGP.AbstractSyntaxTree.SimilarityEvaluation.TreeEditDistance {
	// Based on Java implementation: https://github.com/ijkilchenko/ZhangShasha
	// Publication: https://pdfs.semanticscholar.org/277f/f0c74cc72663d0aabbeae25a3e97b245457c.pdf?_ga=2.230165084.1186213.1577988064-575023686.1577988064
	class TestMain {
		public static void Main(string[] args) {

			Problem[] simpleProblems = new Problem[] {
				// Simple Problems
				new AllEqualProblem(3),			// 0
				new ContainsFirstProblem(3),	// 1
				new CountZeroesProblem(3),		// 2
				new IsOrderedProblem(3),		// 3
				new MajorityProblem(3),			// 4
				new MaximumProblem(3),			// 5
				new AllEqualProblem(4),			// 6
				new ContainsFirstProblem(4),	// 7
				new CountZeroesProblem(4),		// 8
				new IsOrderedProblem(4),		// 9
				new MajorityProblem(4),			// 10
				new MaximumProblem(4)			// 11
			};

			var similarity = new TreeDistanceSimilarity();
			foreach (var problem in simpleProblems) {
				var solutions = problem.GetOptimalSolutions();
				Console.WriteLine("Problem: " + problem.GetType().Name);
				foreach (var solution in solutions) {
					//Console.WriteLine(solution.ToString());
					var sim = similarity.Measure(solution, solution);   // should be 0
					Console.WriteLine("Similarity with self: " + sim);
				}

			}

			/*
			// Sample trees (in preorder).
			string tree1Str1 = "f(d(a c(b)) e)";
			string tree1Str2 = "f(c(d(a b)) e)";
			// Distance: 2 (main example used in the Zhang-Shasha paper)

			string tree1Str3 = "a(b(c d) e(f g(i)))";
			string tree1Str4 = "a(b(c d) e(f g(h)))";
			// Distance: 1

			string tree1Str5 = "d";
			string tree1Str6 = "g(h)";
			// Distance: 2

			Tree tree1 = new Tree(tree1Str1);
			Tree tree2 = new Tree(tree1Str2);

			Tree tree3 = new Tree(tree1Str3);
			Tree tree4 = new Tree(tree1Str4);

			Tree tree5 = new Tree(tree1Str5);
			Tree tree6 = new Tree(tree1Str6);

			int distance1 = Tree.ZhangShasha(tree1, tree2);
			Console.WriteLine("Expected 2; got " + distance1);

			int distance2 = Tree.ZhangShasha(tree3, tree4);
			Console.WriteLine("Expected 1; got " + distance2);

			int distance3 = Tree.ZhangShasha(tree5, tree6);
			Console.WriteLine("Expected 2; got " + distance3);*/

			Console.ReadKey();
		}
	}
}
