using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.GP.Problems;
using PatternGuidedGP.GP.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Evaluators {
	interface IFitnessEvaluator {
		IFitnessCalculator FitnessCalculator { get; set; }
		FitnessResult Evaluate(Individual individual, Problem problem);
		void OnEvaluationFinished();
		void OnStartEvaluation();
	}
}
