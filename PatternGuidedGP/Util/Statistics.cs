using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Util {
	class Statistics {
		private static Statistics _instance = new Statistics();
		public static Statistics Instance {
			get {
				return _instance;
			}
		}

		// count node types (if, for, i, ret etc.) in population
		private Dictionary<string, int> _nodeTypeCount = new Dictionary<string, int>();

		// count (successful) backpropagation attempts in order to calculate success rate
		int _backpropagationAttemptsCrossover = 0;	// total backpropagation attempts
		int _backpropagationSuccessCrossover = 0;   // backpropagation success (without ambiguous results, failure)
		double _backpropagationFitnessChangeCrossover = 0;
		double _backpropagationFitnessBestChangeCrossover = 0;
		double _backpropagationFitnessWorstChangeCrossover = 0;
		int _backpropagationAttemptsMutation = 0;  // total backpropagation attempts
		int _backpropagationSuccessMutation = 0;   // backpropagation success (without ambiguous results, failure)
		double _backpropagationFitnessChangeMutation = 0;
		double _backpropagationFitnessBestChangeMutation = 0;
		double _backpropagationFitnessWorstChangeMutation = 0;

		// count (successful) replace attempts in record-based mutation in order to calculate success rate
		int _recordReplaceAttempts = 0;	// total replace attempts
		int _recordReplaceSuccess = 0;	// individual is better after replace
		int _recordReplaceFailure = 0;  // individual is worse after replace
		double _recordReplaceChange = 0.0;  // fitness change
		double _recordReplaceBestChange = 0.0;  // best fitness change
		double _recordReplaceWorstChange = 0.0;  // worst fitness change

		public string ToString(params string[] nodeTypes) {
			int[] counts = GetNodeTypeCount(nodeTypes);
			StringBuilder builder = new StringBuilder();
			foreach (int count in counts) {
				builder.Append(count + ";");
			}
			builder.Append(_backpropagationAttemptsCrossover + ";");
			builder.Append(_backpropagationSuccessCrossover + ";");
			builder.Append(_backpropagationFitnessChangeCrossover + ";");
			builder.Append(_backpropagationFitnessBestChangeCrossover + ";");
			builder.Append(_backpropagationFitnessWorstChangeCrossover + ";");
			builder.Append(_backpropagationAttemptsMutation + ";");
			builder.Append(_backpropagationSuccessMutation + ";");
			builder.Append(_backpropagationFitnessChangeCrossover + ";");
			builder.Append(_backpropagationFitnessBestChangeCrossover + ";");
			builder.Append(_backpropagationFitnessWorstChangeCrossover + ";");

			builder.Append(_recordReplaceAttempts + ";");
			builder.Append(_recordReplaceSuccess + ";");
			builder.Append(_recordReplaceFailure + ";");
			builder.Append(_recordReplaceChange + ";");
			builder.Append(_recordReplaceBestChange + ";");
			builder.Append(_recordReplaceWorstChange);
			return builder.ToString();
		}

		// record-based replacement
		// ========================
		public void AddRecordReplaceAttempt(double fitnessChange) {
			_recordReplaceAttempts++;
			_recordReplaceChange += fitnessChange;
			if (fitnessChange < 0) {
				_recordReplaceSuccess++;
				if (fitnessChange < _recordReplaceBestChange) {
					_recordReplaceBestChange = fitnessChange;
				}
			} else if (fitnessChange > 0) {
				_recordReplaceFailure++;
				if (fitnessChange > _recordReplaceWorstChange) {
					_recordReplaceWorstChange = fitnessChange;
				}
			}
		}

		public void ClearRecordReplaceAttempts() {
			_recordReplaceAttempts = 0;
			_recordReplaceChange = 0;
			_recordReplaceFailure = 0;
			_recordReplaceSuccess = 0;
			_recordReplaceBestChange = 0;
			_recordReplaceWorstChange = 0;
		}


		// backpropagation
		// ===============
		public void AddBackpropagationAttemptCrossover(bool successful, double fitnessChange) {
			_backpropagationAttemptsCrossover++;
			if (successful) {
				_backpropagationSuccessCrossover++;
				_backpropagationFitnessChangeCrossover += fitnessChange;
				if (fitnessChange < _backpropagationFitnessBestChangeCrossover) {
					_backpropagationFitnessBestChangeCrossover = fitnessChange;
				}
				if (fitnessChange > _backpropagationFitnessWorstChangeCrossover) {
					_backpropagationFitnessWorstChangeCrossover = fitnessChange;
				}
			}
		}

		public void AddBackpropagationAttemptMutation(bool successful, double fitnessChange) {
			_backpropagationAttemptsMutation++;
			if (successful) {
				_backpropagationSuccessMutation++;
				_backpropagationFitnessChangeMutation += fitnessChange;
				if (fitnessChange < _backpropagationFitnessBestChangeMutation) {
					_backpropagationFitnessBestChangeMutation = fitnessChange;
				}
				if (fitnessChange > _backpropagationFitnessWorstChangeMutation) {
					_backpropagationFitnessWorstChangeMutation = fitnessChange;
				}
			}
		}

		public void ClearBackpropagationAttempts() {
			_backpropagationSuccessCrossover = 0;
			_backpropagationAttemptsCrossover = 0;
			_backpropagationFitnessChangeCrossover = 0;
			_backpropagationFitnessBestChangeCrossover = 0;
			_backpropagationFitnessWorstChangeCrossover = 0;
			_backpropagationSuccessMutation = 0;
			_backpropagationAttemptsMutation = 0;
			_backpropagationFitnessChangeMutation = 0;
			_backpropagationFitnessBestChangeMutation = 0;
			_backpropagationFitnessWorstChangeMutation = 0;
		}

		// node type count
		// ===============
		public void AddNodeType(string type) {
			if (_nodeTypeCount.ContainsKey(type)) {
				_nodeTypeCount[type]++;
			} else {
				_nodeTypeCount.Add(type, 1);
			}
		}

		public void ClearNodeTypes() {
			_nodeTypeCount.Clear();
		}

		public int[] GetNodeTypeCount(params string [] types) {
			int[] counts = new int[types.Length];
			for (int i = 0; i < types.Length; i++) {
				string type = types[i];
				if (type == "total") {
					counts[i] = _nodeTypeCount.Values.Sum();
				} else {
					if (_nodeTypeCount.ContainsKey(type)) {
						counts[i] = _nodeTypeCount[type];
					} else {
						counts[i] = 0;
					}
				}
			}
			return counts;
		}

		public void ClearAll() {
			ClearNodeTypes();
			ClearBackpropagationAttempts();
			ClearRecordReplaceAttempts();
		}
	}
}
