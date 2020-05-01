using Microsoft.CodeAnalysis.CSharp;
using PatternGuidedGP.AbstractSyntaxTree.SyntaxGenerator.CSharp;
using PatternGuidedGP.GP.SemanticGP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	abstract class Expression<TResult> : TreeNode, ISemanticsHolder, IInvertible {
		public override Type Type => typeof(TResult);

		private Semantics _semantics;
		public Semantics Semantics {
			get => _semantics;
			set {
				_semantics = value;
				_isSemanticsEvaluated = true;
			}
		}

		private bool _isSemanticsEvaluated;
		public bool SemanticsEvaluated {
			get => _isSemanticsEvaluated;
			set => _isSemanticsEvaluated = value;
		}

		public virtual bool IsInvertible { get; } = false;

		public virtual object GetComplementValue(int k, int semanticsIndex) {
			return null;
		}

		public virtual IEnumerable<object> Invert(object desiredValue, int k, object complementValue, out bool ambiguous) {
			ambiguous = false;
			return null;
		}
	}
}
