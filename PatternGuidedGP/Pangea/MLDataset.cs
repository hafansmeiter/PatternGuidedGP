using PatternGuidedGP.GP;
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

		public object[] this[ulong feature] {
			get {
				return _features[feature];
			}
		}

		public static MLDataset FromExecutionTraces(Individual /* unused */ individual, IList<ExecutionTrace> traces) {
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

		public int[][] ToRawInputDataset(bool removeConstantFeatures = true) {
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

			// Remove constant features, as they are apparently not allowed
			// in Accord.net decision trees. DecisionTree.cs:125
			// https://github.com/accord-net/framework/blob/792015d0e2ee250228dfafb99ea0e84d031a29ae/Sources/Accord.MachineLearning/DecisionTrees/DecisionTree.cs
			if (removeConstantFeatures) {
				var constantColumns = GetConstantColumns(dataset);
				RemoveFeatures(dataset, constantColumns);
			}
			return dataset;
		}

		private IEnumerable<int> GetConstantColumns(int[][] dataset) {
			IList<int> constantColumns = new List<int>();
			for (int i = 0; i < _features.Count; i++) {
				bool isConstant = true;
				for (int j = 0; j < Count - 1; j++) {
					if (dataset[j][i] != dataset[j + 1][i]) {
						isConstant = false;
						break;
					}
				}
				if (isConstant) {
					constantColumns.Add(i);
				}
			}
			return constantColumns;
		}

		private int[][] RemoveFeatures(int[][] dataset, IEnumerable<int> featureIndices) {
			int[][] newDataset = new int[Count][];
			int featureCount = _features.Count;
			for (int i = 0; i < Count; i++) {
				newDataset[i] = new int[featureCount - featureIndices.Count()];
				int featuresRemoved = 0;
				for (int j = 0; j < featureCount; j++) {
					if (!featureIndices.Contains(j)) {
						newDataset[i][j - featuresRemoved] = dataset[i][j];
					} else {
						featuresRemoved++;
					}
				}
			}
			return newDataset;
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
