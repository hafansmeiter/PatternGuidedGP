using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP {
	class Individual {
		public double Fitness { get; set; }
		public bool FitnessEvaluated { get; set; }
		public SyntaxNode SyntaxRoot { get; set; }
	}
}
