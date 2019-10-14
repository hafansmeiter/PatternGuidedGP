using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Pangea {
	class MLDataset {
		private SortedDictionary<ulong, object []> _features 
			= new SortedDictionary<ulong, object []>();

		public int Count { get; private set; }
		public IEnumerable<ulong> Features {
			get {
				return _features.Keys;
			}
		}

		public static MLDataset FromExecutionTraces(IList<ExecutionTrace> traces) {
			MLDataset dataset = new MLDataset();
			int count = traces.Count;
			dataset.Count = count;
			for (int i = 0; i < count; i++) {
				var trace = traces[i];
				foreach (var entry in trace.Entries) {
					foreach (var record in entry.Records) {
						object[] featureValues;
						if (!dataset._features.TryGetValue(record.NodeId, out featureValues)) {
							featureValues = new object[count];
							dataset._features.Add(record.NodeId, featureValues);
						}
						featureValues[i] = record.Value;
					}
				}
			}
			return dataset;
		}

		public int[][] ToRawInputDataset() {
			// First, remove constant features, as they are apparently not allowed
			// in Accord.net decision trees. DecisionTree.cs:125
			// https://github.com/accord-net/framework/blob/792015d0e2ee250228dfafb99ea0e84d031a29ae/Sources/Accord.MachineLearning/DecisionTrees/DecisionTree.cs
			// RemoveConstantFeatures(); 
			// ==> call moved to MDLFitnessEvaluator
			int[][] dataset = new int[Count][];
			int featureCount = _features.Count;
			for (int i = 0; i < Count; i++) {
				dataset[i] = new int[featureCount];
				for (int j = 0; j < featureCount; j++) {
					var feature = _features.Keys.ElementAt(j);
					var featureValue = _features[feature][i];
					dataset[i][j] = ToDatasetValue(featureValue);
				}
			}
			return dataset;
		}

		public void RemoveConstantFeatures() {
			int featureCount = _features.Count;
			var toRemove = new List<ulong>();
			for (int i = 0; i < featureCount; i++) {
				var feature = _features.Keys.ElementAt(i);
				var featureValue = ToDatasetValue(_features[feature][0]);
				bool allEqual = true;
				for (int j = 1; j < Count; j++) {
					if (featureValue != ToDatasetValue(_features[feature][j])) {
						allEqual = false;
						break;
					}
				}
				if (allEqual) {
					toRemove.Add(feature);
				}
			}
			foreach (var feature in toRemove) {
				_features.Remove(feature);
			}
		}

		public static int ToDatasetValue(object o) {
			int value = -1;
			if (o != null) {
				if (o.GetType() == typeof(int)) {
					value = (int)o;
				}
				else if (o.GetType() == typeof(bool)) {
					value = (bool)o ? 1 : 0;
				}
			}
			return value;
		}
	}
}
