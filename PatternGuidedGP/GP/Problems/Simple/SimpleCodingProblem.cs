﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.GP.Evaluators;
using PatternGuidedGP.GP.SemanticGP;
using PatternGuidedGP.GP.Tests;

namespace PatternGuidedGP.GP.Problems.Simple {
	abstract class SimpleCodingProblem : CodingProblem {
		public abstract Type ParameterType { get; }
		public override IFitnessCalculator FitnessCalculator => new EqualityFitnessCalculator();

		public SimpleCodingProblem(int n, bool initialize = true) : base(n, initialize) {
			GeometricCalculator = GetGeometricCalculator();
		}

		protected override void GetInstructionSet(InstructionSetBuilder builder) {
			base.GetInstructionSet(builder);
			if (ParameterType == typeof(int)) {
				builder.AddIntParameters(ParameterCount);
			}
			else if (ParameterType == typeof(bool)) {
				builder.AddBoolParameters(ParameterCount);
			}
			if (ReturnType == typeof(int) || ParameterType == typeof(int)) {
				//builder.AddIntegerDomain();
				builder.AddIntRandomLiteral(1, ParameterCount);
				//builder.AddForLoopTimesStatement();
			}
			builder.AddSimpleBooleanDomain();
			builder.AddIfStatement();
		}

		protected override void GetCodeTemplate(CodeTemplateBuilder builder) {
			base.GetCodeTemplate(builder);
			builder.UseParameterType(ParameterType, ParameterCount);
		}
	}
}
