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
		private static string LOG_PATH = ".\\";

		static void Main(string[] args) {
			int runConfig, runProblem, fromConfig = 0, fromProblem = 0, steps = 0;
			string problemSet;
			EvaluateArgs(args, out runConfig, out runProblem, out fromConfig, out fromProblem, out problemSet, out steps);
			MDLFitnessCalculator.STEPS = steps;
			Logger.Level = 0;

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
					/*new AllEqualProblem(4),			// 6
					new ContainsFirstProblem(4),	// 7
					new CountZeroesProblem(4),		// 8
					new IsOrderedProblem(4),		// 9
					new MajorityProblem(4),			// 10
					new MaximumProblem(4),			// 11*/
					/*new CompareProblem(6),			// 6
					new MultiplexerProblem(6, 2),	// 7
					new ParityProblem(6)            // 8*/
				};
				RunProblems(simpleProblems, runConfig, runProblem, fromConfig, fromProblem);
			} else if (problemSet == "advanced") {
				SyntaxConfiguration.Current = new SyntaxConfiguration.Simple();
				//SyntaxConfiguration.Current = new SyntaxConfiguration.Advanced();
				Problem[] advancedProblems = new Problem[] {
					// Advanced Problems
					new MedianProblem(),				// 0
					new CountOddsProblem(),				// 1
					new LastIndexOfZeroProblem(),		// 2
					new SmallOrLargeProblem()			// 3
				};
				RunProblems(advancedProblems, runConfig, runProblem, fromConfig, fromProblem);
			}
		}

		static void RunProblems(Problem [] problems, int runConfig, int runProblem, int fromConfig, int fromProblem) {
			int maxTreeDepth = 7;
			int maxInitialTreeDepth = 4;
			int maxMutationTreeDepth = 3;

			var semanticsBasedSubTreePool = new SemanticsBasedSubTreePool();
			var recordBasedSubTreePool = new RecordBasedSubTreePool();
			var generator = new KozaTreeGeneratorFull();
			var evaluatingSubtreeMutator = new EvaluatingRandomSubtreeMutator(recordBasedSubTreePool, maxTreeDepth, maxTreeDepth);

			var configurations = new[] {
				// /config:0
				new RunConfiguration("Standard GP") {
					FitnessEvaluator = new ProgramFitnessEvaluator(),
					Crossover = new RandomSubtreeCrossover(maxTreeDepth),
					Mutator = new RandomSubtreeMutator(generator, maxTreeDepth, maxMutationTreeDepth)
				},
				// /config:1
				new RunConfiguration("Standard PANGEA") {
					FitnessEvaluator = new MDLFitnessEvaluator(),
					Crossover = new RandomSubtreeCrossover(maxTreeDepth),
					Mutator = new RandomSubtreeMutator(generator, maxTreeDepth, maxMutationTreeDepth)
				},
				// /config:2
				/*new RunConfiguration("PANGEA + Semantic Backpropagation Operators") {
					FitnessEvaluator = new SemanticMDLFitnessEvaluator() {
						SubTreePool = semanticsBasedSubTreePool
					},
					Crossover = new ApproximatelyGeometricSemanticCrossover(semanticsBasedSubTreePool, maxTreeDepth),
					Mutator = new ApproximatelySemanticMutator(semanticsBasedSubTreePool, maxTreeDepth)
				},*/
				// /config:2
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
				// /config:4
				/*new RunConfiguration("PANGEA + Multi Random Operators (Sem. Backprop. + Random Subtree)") {
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
				},*/
				// /config:5
				/*new RunConfiguration("PANGEA + Record-Based Subtree Pool") {
					FitnessEvaluator = new MDLFitnessEvaluator() {
						SubTreePool = recordBasedSubTreePool
					},
					Crossover = new RandomSubtreeCrossover(maxTreeDepth),
					Mutator = new RandomSubtreeMutator(recordBasedSubTreePool, maxTreeDepth, maxTreeDepth)
				},*/
				// /config:3
				new RunConfiguration("PANGEA + Multi Random Mutator (Record-Based Subtree Pool + Random Subtree)") {
					FitnessEvaluator = new MDLFitnessEvaluator() {
						SubTreePool = recordBasedSubTreePool
					},
					Crossover = new RandomSubtreeCrossover(maxTreeDepth),
					Mutator = new MultiRandomMutator() {
						Options = {
							evaluatingSubtreeMutator,
							new RandomSubtreeMutator(generator, maxTreeDepth, maxMutationTreeDepth)
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
					" (Population: " + config.PopulationSize + ", generations: " + config.Generations 
					+ ", runs: " + config.Runs + ", trace steps: " + MDLFitnessCalculator.STEPS + ")");
				for (int j = fromProblem; j < problems.Length; j++) {
					var problem = problems[j];
					Logger.WriteLine(0, problem.GetType().Name + " (" + problem.ParameterCount + "):");
					generator.InstructionSetRepository = problem.InstructionSetRepository;

					// set problem specific parts
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
						if ((((MultiRandomMutator)config.Mutator).Options[0]) is ISemanticOperator) {
							((ISemanticOperator)((MultiRandomMutator)config.Mutator).Options[0]).DesiredSemantics
								= problem.TestSuite.Semantics;
						}
						else if ((((MultiRandomMutator)config.Mutator).Options[0]) is EvaluatingRandomSubtreeMutator) {
							((EvaluatingRandomSubtreeMutator)((MultiRandomMutator)config.Mutator).Options[0]).FitnessEvaluator = config.FitnessEvaluator;
							((EvaluatingRandomSubtreeMutator)((MultiRandomMutator)config.Mutator).Options[0]).Problem = problem;
						}
					}
					if (config.FitnessEvaluator is MDLFitnessEvaluator) {
						(((MDLFitnessCalculator)((MDLFitnessEvaluator)config.FitnessEvaluator).FitnessCalculator)).StandardFitnessCalculator = problem.FitnessCalculator;
					} else {
						((ProgramFitnessEvaluator)config.FitnessEvaluator).FitnessCalculator = problem.FitnessCalculator;
					}
					problem.FitnessEvaluator = config.FitnessEvaluator;

					int solved = 0;
					for (int i = 0; i < config.Runs; i++) {
						semanticsBasedSubTreePool.Clear();
						recordBasedSubTreePool.Clear();

						DefaultAlgorithm algorithm = new DefaultAlgorithm(config.PopulationSize, config.Generations, true) {
							Crossover = config.Crossover,
							CrossoverRate = 0.7,
							Elitism = 5,
							Initializer = new RampedHalfHalfInitializer(maxInitialTreeDepth, problem.InstructionSetRepository),
							MaxTreeDepth = maxTreeDepth,   // not used; set individually in mutators and tree generators
							MutationRate = 0.2,
							Mutator = config.Mutator,
							Selector = new TournamentSelector(7)
						};

						Individual bestSolution = algorithm.Run(problem);
						if (algorithm.IsSolutionFound()) {
							solved++;
							Logger.WriteLine(0, "Solution found: \n" + bestSolution.ToString());
						} else {
							Logger.WriteLine(0, "No Solution found. Best solution: \n" + bestSolution.ToString());
						}
					}
					Logger.WriteLine(0, problem.GetType().Name + ": Solved " + solved + "/" + config.Runs);
					Logger.WriteLine(0, "=====================================================");
				}
			}
		}

		private static void EvaluateArgs(string [] args, out int runConfig, out int runProblem, 
			out int fromConfig, out int fromProblem, out string problemSet, out int steps) {
			runConfig = -1;
			runProblem = -1;
			fromConfig = 0;
			fromProblem = 0;
			problemSet = "";
			steps = 5;
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
				} else if (arg.StartsWith("/logPath:")) {
					LOG_PATH = arg.Substring(9);
				} else if (arg.StartsWith("/steps:")) {
					steps = int.Parse(arg.Substring(7));
				}
			}
		}

		private static string GetLogFilename(RunConfiguration config) {
			return LOG_PATH + (config.Name + "_" + GetTimestamp()).Replace(' ', '_').Replace('.', '-') + ".txt";
		}

		private static string GetTimestamp() {
			return DateTime.Now.ToShortDateString();
		}
	}
}
