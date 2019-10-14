using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Pangea {
	public class ExecutionRecord {
		public ulong NodeId { get; }
		public object Value { get; }

		public ExecutionRecord(ulong nodeId, object value) {
			NodeId = nodeId;
			Value = value;
		}

		public override string ToString() {
			return "[" + NodeId + " => " + Value + "]";
		}
	}
}
