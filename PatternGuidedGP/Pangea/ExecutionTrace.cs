using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Pangea {

	public class ExecutionTrace : MarshalByRefObject {

		IList<ExecutionState> _states;
		public IList<ExecutionState> States {
			get {
				return _states;
			}
		}

		public ExecutionTrace() {
			_states = new List<ExecutionState>();
		}

		public void Add(ulong nodeId, ulong recordNodeId, object recordValue, int operatorId) {
			_states.Add(new ExecutionState(recordNodeId, recordValue, operatorId));
		}
	}
}
