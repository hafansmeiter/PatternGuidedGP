using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Pangea {
	public class ExecutionTraces {
		private static IList<ExecutionTrace> _traces { get; } = new List<ExecutionTrace>();
		private static ExecutionTrace _current;
		private static ExecutionTrace _emptyExecutionTrace = new ExecutionTrace();

		public static ExecutionTrace Current {
			get {
				if (_current == null) {
					_current = new ExecutionTrace();
				}
				return _current;
			}
		}

		public static IList<ExecutionTrace> Traces {
			get {
				return _traces;
			}
		}

		public static void Reset() {
			_current = null;
			_traces.Clear();
		}

		public static void FinishCurrent() {
			if (_current == null) {
				_current = _emptyExecutionTrace;
			}
			_traces.Add(_current);
			_current = null;
		}
	}
}
