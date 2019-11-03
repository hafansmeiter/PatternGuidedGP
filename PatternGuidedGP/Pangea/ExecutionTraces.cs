using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Pangea {
	 
	public class ExecutionTraces : MarshalByRefObject {
		private IList<ExecutionTrace> _traces { get; } = new List<ExecutionTrace>();
		private ExecutionTrace _current;
		private ExecutionTrace _emptyExecutionTrace = new ExecutionTrace();

		public ExecutionTraces() {
		}

		public ExecutionTrace Current {
			get {
				if (_current == null) {
					_current = new ExecutionTrace();
				}
				return _current;
			}
		}

		public IList<ExecutionTrace> Traces {
			get {
				return _traces;
			}
		}

		public void Reset() {
			_current = null;
			_traces.Clear();
		}

		public void FinishCurrent() {
			if (_current == null) {
				_current = _emptyExecutionTrace;
			}
			_traces.Add(_current);
			_current = null;
		}
	}
}
