using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math.Optimization.Losses;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.Pool;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.Compiler.CSharp;
using PatternGuidedGP.GP;
using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.GP.Operators;
using PatternGuidedGP.GP.Problems;
using PatternGuidedGP.GP.Problems.Advanced;
using PatternGuidedGP.GP.Problems.Simple;
using PatternGuidedGP.GP.SemanticGP;
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Pangea;
using PatternGuidedGP.Util;

namespace PatternGuidedGP {
	class Program {
		private static string LOG_PATH = "..\\..\\..\\Logs\\";

		static void Main(string[] args) {
			int runConfig, runProblem, fromConfig = 0, fromProblem = 0;
			string problemSet;
			EvaluateArgs(args, out runConfig, out runProblem, out fromConfig, out fromProblem, out problemSet);
			Logger.Level = 4;

			if (problemSet == "simple") {
				SyntaxConfiguration.Current = new SyntaxConfiguration.Simple();
				Problem[] simpleProblems = new Problem[] {
					// Simple Problems
					new AllEqualProblem(3),			// 0
					new ContainsFirstProblem(3),	// 1
					new CountZeroesProblem(3),		// 2
					new IsOrderedProblem(3),		// 3
					new MajorityProblem(3),			// 4
					new MaximumProblem(3),			// 5
					new CompareProblem(6),			// 6
					new MultiplexerProblem(6, 2),	// 7
					new ParityProblem(6)            // 8
				};
				RunProblems(simpleProblems, runConfig, runProblem, fromConfig, fromProblem);
			} else if (problemSet == "advanced") {
				SyntaxConfiguration.Current = new SyntaxConfiguration.Simple();
				Problem[] advancedProblems = new Problem[] {
					// Advanced Problems
					new AverageProblem(),			// 0
					new MedianProblem()				// 1
				};
				RunProblems(advancedProblems, runConfig, runProblem, fromConfig, fromProblem);
			}
		}

		static void RunProblems(Problem [] problems, int runConfig, int runProblem, int fromConfig, int fromProblem) {
			int maxTreeDepth = 7;
			int maxMutationTreeDepth = 3;

			var semanticsBasedSubTreePool = new SemanticsBasedSubTreePool();
			var recordBasedSubTreePool = new RecordBasedSubTreePool();
			var generator = new KozaTreeGeneratorFull();

			var configurations = new[] {
				new RunConfiguration("Standard GP") {
					FitnessEvaluator = new ProgramFitnessEvaluator(),
					Crossover = new RandomSubtreeCrossover(maxTreeDepth),
					Mutator = new RandomSubtreeMutator(generator, maxTreeDepth, maxMutationTreeDepth)
				},
				new RunConfiguration("Standard PANGEA") {
					FitnessEvaluator = new MDLFitnessEvaluator(),
					Crossover = new RandomSubtreeCrossover(maxTreeDepth),
					Mutator = new RandomSubtreeMutator(generator, maxTreeDepth, maxMutationTreeDepth)
				},
				new RunConfiguration("PANGEA + Semantic Backpropagation Operators") {
					FitnessEvaluator = new SemanticMDLFitnessEvaluator() {
						SubTreePool = semanticsBasedSubTreePool
					},
					Crossover = new ApproximatelyGeometricSemanticCrossover(semanticsBasedSubTreePool, maxTreeDepth),
					Mutator = new ApproximatelySemanticMutator(semanticsBasedSubTreePool, maxTreeDepth)
				},
				new RunConfiguration("PANGEA + Semantic Backpropagation Operators + Fallback Random Operators") {
					FitnessEvaluator = new SemanticMDLFitnessEvaluator() {
						SubTreePool = semanticsBasedSubTreePool
					},
					Crossover = new ApproximatelyGeometricSemanticCrossover(semanticsBasedSubTreePool, maxTreeDepth) {
						Fallback = new RandomSubtreeCrossover(maxTreeDepth)
					},
					Mutator = new ApproximatelySemanticMutator(semanticsBasedSubTreePool, maxTreeDepth) {
						Fallback = new RandomSubtreeMutator(generator, maxTreeDepth, maxMutationTreeDepth)
					}
				},
				new RunConfiguration("PANGEA + Multi Random Operators (Sem. Backprop. + Random Subtree)") {
					FitnessEvaluator = new SemanticMDLFitnessEvaluator() {
						SubTreePool = semanticsBasedSubTreePool
					},
					Crossover = new MultiRandomCrossover() {
						Options = {
							new ApproximatelyGeometricSemanticCrossover(semanticsBasedSubTreePool, maxTreeDepth),
							new RandomSubtreeCrossover(maxTreeDepth)
						}
					},
					Mutator = new MultiRandomMutator() {
						Options = {
							new ApproximatelySemanticMutator(semanticsBasedSubTreePool, maxTreeDepth),
							new RandomSubtreeMutator(generator, maxTreeDepth, maxMutationTreeDepth)
						}
					},
				},
				new RunConfiguration("PANGEA + Record-Based Subtree Pool") {
					FitnessEvaluator = new MDLFitnessEvaluator() {
						SubTreePool = recordBasedSubTreePool
					},
					Crossover = new RandomSubtreeCrossover(maxTreeDepth),
					Mutator = new RandomSubtreeMutator(recordBasedSubTreePool, maxTreeDepth, maxTreeDepth)
				},
				new RunConfiguration("PANGEA + Multi Random Mutator (Record-Based Subtree Pool + Random Subtree)") {
					FitnessEvaluator = new MDLFitnessEvaluator() {
						SubTreePool = recordBasedSubTreePool
					},
					Crossover = new RandomSubtreeCrossover(maxTreeDepth),
					Mutator = new MultiRandomMutator() {
						Options = {
							new RandomSubtreeMutator(generator, maxTreeDepth, maxMutationTreeDepth),
							new RandomSubtreeMutator(recordBasedSubTreePool, maxTreeDepth, maxTreeDepth)
						}
					}
				}
			};

			if (runConfig >= 0) {
				configurations = new[] { configurations[runConfig] };
			}
			if (runProblem >= 0) {
				problems = new[] { problems[runProblem] };
			}

			foreach (var config in configurations) {
				Logger.FileName = GetLogFilename(config);
				Logger.WriteLine(0, "Run configuration: " + config.Name + 
					" (Population: " + config.PopulationSize + ", generations: " + config.Generations);
				for (int j = fromProblem; j < problems.Length - fromProblem; j++) {
					var problem = problems[j];
					Logger.WriteLine(0, problem.GetType().Name + ":");
					problem.FitnessEvaluator = config.FitnessEvaluator;
					generator.InstructionSetRepository = problem.InstructionSetRepository;
					if (config.Crossover is IGeometricOperator) {
						((IGeometricOperator)config.Crossover).GeometricCalculator = problem.GeometricCalculator;
					}
					if (config.Crossover is MultiRandomCrossover) {
						((IGeometricOperator) ((MultiRandomCrossover)config.Crossover).Options[0]).GeometricCalculator 
							= problem.GeometricCalculator;
					}
					if (config.Mutator is ISemanticOperator) {
						((ISemanticOperator)config.Mutator).DesiredSemantics = problem.TestSuite.Semantics;
					}
					if (config.Mutator is MultiRandomMutator) {
						((ISemanticOperator)((MultiRandomMutator)config.Mutator).Options[0]).DesiredSemantics 
							= problem.TestSuite.Semantics;
					}


					int solved = 0;
					for (int i = 0; i < config.Runs; i++) {
						semanticsBasedSubTreePool.Clear();
						recordBasedSubTreePool.Clear();

						DefaultAlgorithm algorithm = new DefaultAlgorithm(config.PopulationSize, config.Generations, false) {
							Crossover = config.Crossover,
							CrossoverRate = 0.8,
							Elitism = 5,
							Initializer = new RampedHalfHalfInitializer(maxTreeDepth, problem.InstructionSetRepository),
							MaxTreeDepth = 9,   // not used
							MutationRate = 0.2,
							Mutator = config.Mutator,
							Selector = new TournamentSelector(7)
						};

						Individual bestSolution = algorithm.Run(problem);
						if (algorithm.IsSolutionFound()) {
							solved++;
						}
					}
					Logger.WriteLine(0, problem.GetType().Name + ": Solved " + solved + "/" + config.Runs);
					Logger.WriteLine(0, "=====================================================");
				}
			}
		}

		private static void EvaluateArgs(string [] args, out int runConfig, out int runProblem, 
			out int fromConfig, out int fromProblem, out string problemSet) {
			runConfig = -1;
			runProblem = -1;
			fromConfig = 0;
			fromProblem = 0;
			problemSet = "";
			foreach (var arg in args) {
				if (arg.StartsWith("/config:")) {
					runConfig = int.Parse(arg.Substring(8));
				} else if (arg.StartsWith("/problem:")) {
					runProblem = int.Parse(arg.Substring(9));
				} else if (arg.StartsWith("/fromConfig:")) {
					fromConfig = int.Parse(arg.Substring(12));
				} else if (arg.StartsWith("/fromProblem:")) {
					fromProblem = int.Parse(arg.Substring(13));
				} else if (arg.StartsWith("/problemSet:")) {
					problemSet = arg.Substring(12);
				}
			}
		}

		private static string GetLogFilename(RunConfiguration config) {
			return LOG_PATH + (config.Name + GetTimestamp()).Replace(' ', '_').Replace('.', '-') + ".txt";
		}

		private static string GetTimestamp() {
			return DateTime.Now.ToShortDateString();
		}
	}
}
