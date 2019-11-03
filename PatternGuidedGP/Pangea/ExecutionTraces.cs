using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Pangea {
	 
	public class ExecutionTraces : MarshalByRefObject {
		static private IList<ExecutionTrace> _traces { get; } = new List<ExecutionTrace>();
		static private ExecutionTrace _current;
		static private ExecutionTrace _emptyExecutionTrace = new ExecutionTrace();

		static public ExecutionTrace Current {
			get {
				if (_current == null) {
					_current = new ExecutionTrace();
				}
				return _current;
			}
		}

		static public IList<ExecutionTrace> Traces {
			get {
				return _traces;
			}
		}

		static public void Reset() {
			_current = null;
			_traces.Clear();
		}

		static public void FinishCurrent() {
			if (_current == null) {
				_current = _emptyExecutionTrace;
			}
			_traces.Add(_current);
			_current = null;
		}
	}
}
