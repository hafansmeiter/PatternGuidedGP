using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternGuidedGP.GP.Tests;

namespace PatternGuidedGP.GP.Problems.Simple {
	abstract class SimpleCodingProblem : CodingProblem {
		public abstract Type ParameterType { get; }

		public SimpleCodingProblem(int n, bool initialize = true) : base(n, initialize) {
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
				builder.AddIntegerDomain();
				builder.AddForCountStatement();
			}
			builder.AddBooleanDomain();
			builder.AddIfStatement();
		}

		protected override void GetCodeTemplate(CodeTemplateBuilder builder) {
			base.GetCodeTemplate(builder);
			builder.UseParameterType(ParameterType, ParameterCount);
		}
	}
}
