﻿using System;
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
using PatternGuidedGP.GP.SemanticGP;
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Pangea;
using PatternGuidedGP.Util;

namespace PatternGuidedGP {
	class Program {
		private static string LOG_PATH = "..\\..\\..\\Logs\\";

		static void Main(string[] args) {
			int runConfig, runProblem;
			EvaluateArgs(args, out runConfig, out runProblem);

			Logger.Level = 0;

			int maxTreeDepth = 7;
			int maxMutationTreeDepth = 3;

			var semanticsBasedSubTreePool = new SemanticsBasedSubTreePool();
			var recordBasedSubTreePool = new RecordBasedSubTreePool();
			var generator = new KozaTreeGeneratorGrow();

			var configurations = new[] {
				new RunConfiguration("Standard GP") {
					FitnessEvaluator = new DefaultFitnessEvaluator(),
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

			Problem[] problems = new Problem[] {
				new AllEqualProblem(3),
				new ContainsFirstProblem(3),
				new CountZeroesProblem(3),
				new IsOrderedProblem(3),
				new MajorityProblem(3),
				new MaximumProblem(3),
				new CompareProblem(6),
				new MultiplexerProblem(6, 2),
				new ParityProblem(6)
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
				foreach (var problem in problems) {
					Logger.WriteLine(0, problem.GetType().Name + ":");
					problem.FitnessEvaluator = config.FitnessEvaluator;
					generator.TreeNodeRepository = problem.TreeNodeRepository;
					if (config.Crossover is IGeometricOperator) {
						((IGeometricOperator)config.Crossover).GeometricCalculator = problem.GeometricCalculator;
					}
					if (config.Mutator is ISemanticOperator) {
						((ISemanticOperator)config.Mutator).DesiredSemantics = problem.TestSuite.Semantics;
					}

					int solved = 0;
					for (int i = 0; i < config.Runs; i++) {
						DefaultAlgorithm algorithm = new DefaultAlgorithm(config.PopulationSize, config.Generations) {
							Crossover = config.Crossover,
							CrossoverRate = 0.7,
							Elitism = 5,
							Initializer = new RampedHalfHalfInitializer(maxTreeDepth, problem.TreeNodeRepository),
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

		private static void EvaluateArgs(string [] args, out int runConfig, out int runProblem) {
			runConfig = -1;
			runProblem = -1;
			foreach (var arg in args) {
				if (arg.StartsWith("/config:")) {
					runConfig = int.Parse(arg.Substring(8));
				} else if (arg.StartsWith("/problem:")) {
					runProblem = int.Parse(arg.Substring(9));
				}
			}
		}

		private static string GetLogFilename(RunConfiguration config) {
			return LOG_PATH + (config.Name + GetTimestamp() + ".txt").Replace(' ', '_').Replace('.', '-');
		}

		private static string GetTimestamp() {
			return DateTime.Now.ToShortDateString();
		}
	}
}
