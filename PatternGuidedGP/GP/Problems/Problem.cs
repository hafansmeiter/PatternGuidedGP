using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.GP.SemanticGP;
using PatternGuidedGP.GP.Tests;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Problems {
	abstract class Problem {
		public IFitnessEvaluator FitnessEvaluator { get; set; }
		public TestSuite TestSuite { get; protected set; }
		public CompilationUnitSyntax CodeTemplate { get; protected set; }

		public abstract IFitnessCalculator FitnessCalculator { get; }
		public IGeometricCalculator GeometricCalculator { get; protected set; }

		public IInstructionSetRepository InstructionSetRepository { get; protected set; }
		public abstract Type RootType { get; }
		public abstract Type ReturnType { get; }
		public int ParameterCount { get; set; }
		
		public Problem(bool initialize = true) {
			if (initialize) {
				Initialize();
			}
		}

		public Problem(int n, bool initialize = true) {
			ParameterCount = n;
			if (initialize) {
				Initialize();
			}
		}

		protected void Initialize() {
			TestSuite = GetTestSuite();

			var codeBuilder = new CodeTemplateBuilder();
			GetCodeTemplate(codeBuilder);
			CodeTemplate = codeBuilder.Build();

			var instructionSetBuilder= new InstructionSetBuilder();
			GetInstructionSet(instructionSetBuilder);
			InstructionSetRepository = instructionSetBuilder.Build();

			Logger.WriteLine(4, GetType().Name + " test suite:");
			foreach (var test in TestSuite.TestCases) {
				foreach (var param in test.Parameter) {
					Logger.Write(4, param + " ");
				}
				Logger.WriteLine(4, "-> " + test.Result.ToString());
			}
		}

		protected abstract TestSuite GetTestSuite();

		protected virtual void GetCodeTemplate(CodeTemplateBuilder builder) {
			builder.UseReturnType(ReturnType);
		}

		protected virtual void GetInstructionSet(InstructionSetBuilder builder) {
			if (ReturnType == typeof(int)) {
				builder.AddIntTargetVariable();
			} else if (ReturnType == typeof(bool)) {
				builder.AddBoolTargetVariable();
			} else if (ReturnType == typeof(float)) {
				builder.AddFloatTargetVariable();
			}
		}

		public int Evaluate(Population population) {
			int evaluationCount = 0;
			FitnessEvaluator.FitnessCalculator = FitnessCalculator;
			foreach (var individual in population.Individuals) {
				Logger.WriteLine(4, "Individual tree height: " + individual.SyntaxTree.Height);
				if (!individual.FitnessEvaluated) {
					var result = FitnessEvaluator.Evaluate(individual, this);
					individual.Fitness = result.Fitness;
					evaluationCount++;
				}
			}
			return evaluationCount;
		}
	}
}
