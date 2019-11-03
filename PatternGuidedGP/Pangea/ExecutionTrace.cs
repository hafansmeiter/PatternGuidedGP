using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Pangea {

	[Serializable]
	public class ExecutionTrace {

		[Serializable]
		public class Entry {
			public ulong NodeId { get; set; }
			public IList<ExecutionRecord> Records { get; set; } = new List<ExecutionRecord>();

			public Entry(ulong nodeId, IEnumerable<ExecutionRecord> records) {
				NodeId = nodeId;
				foreach (var record in records) {
					Records.Add(record);
				}
			}
		}

		public IEnumerable<Entry> Entries {
			get {
				return _entries;
			}
		}

		IList<Entry> _entries;

		public ExecutionTrace() {
			_entries = new List<Entry>();
		}

		public void Reset() {
			_entries.Clear();
		}

		public void Add(ulong nodeId, params ExecutionRecord[] traces) {
			Logger.WriteLine(4, "Execution Trace for " + nodeId);
			foreach (var trace in traces) {
				Logger.Write(4, trace.ToString());
			}
			_entries.Add(new Entry(nodeId, traces));
			Logger.WriteLine(4, "");
		}
	}
}
