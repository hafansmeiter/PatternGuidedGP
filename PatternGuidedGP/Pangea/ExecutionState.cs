using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Pangea {

	[Serializable]
	public class ExecutionState {
		public ulong NodeId { get; }
		public object Value { get; }
		public int OperatorId { get; }

		public ExecutionState(ulong nodeId, object value, int operatorId) {
			NodeId = nodeId;
			Value = value;
			OperatorId = operatorId;
		}

		public override string ToString() {
			return "[" + NodeId + " (" + OperatorId + ") => " + Value + "]";
		}
	}
}
