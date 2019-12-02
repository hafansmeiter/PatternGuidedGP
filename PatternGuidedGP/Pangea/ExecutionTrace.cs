using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Pangea {

	public class ExecutionTrace : MarshalByRefObject {

		IList<ExecutionRecord> _records;
		public IList<ExecutionRecord> Records {
			get {
				return _records;
			}
		}

		public ExecutionTrace() {
			_records = new List<ExecutionRecord>();
		}

		public void Add(ulong nodeId, ulong recordNodeId, object recordValue, int operatorId) {
			_records.Add(new ExecutionRecord(recordNodeId, recordValue, operatorId));
		}
	}
}
