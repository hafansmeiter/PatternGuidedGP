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

		private SortedDictionary<ulong, MLDatasetFeature> _featureData
			= new SortedDictionary<ulong, MLDatasetFeature>();

		public int Count { get; private set; }
		public IEnumerable<ulong> Features {
			get {
				return _features.Keys;
			}
		}
		public IEnumerable<MLDatasetFeature> FeatureInfo {
			get {
				return _featureData.Values;
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
							dataset._featureData.Add(record.NodeId, 
								new MLDatasetFeature(record.NodeId, record.Value.GetType()));
						}
						featureValues[i] = record.Value;
					}
				}
			}
			return dataset;
		}

		public int?[][] ToRawInputDataset(bool removeConstantFeatures = true) {
			int?[][] dataset = new int?[Count][];
			int featureCount = _features.Count;
			for (int i = 0; i < Count; i++) {
				dataset[i] = new int?[featureCount];
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
				var constantFeatures = GetConstantFeatures(dataset);
				dataset = RemoveFeatures(dataset, constantFeatures);
			}
			return dataset;
		}

		private IEnumerable<int> GetConstantFeatures(int?[][] dataset) {
			IList<int> constantColumns = new List<int>();
			for (int i = 0; i < _features.Count; i++) {
				bool isConstant = true;
				int value = 0;
				int j = 0;
				// find first not-null value
				for (j = 0; j < Count; j++) {
					if (dataset[j][i].HasValue) {
						value = dataset[j][i].Value;
						break;
					}
				}
				// compare value with rest of the array
				for (j += 1; j < Count; j++) {
					if (dataset[j][i].HasValue && value != dataset[j][i].Value) {
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

		private int?[][] RemoveFeatures(int?[][] dataset, IEnumerable<int> featureIndices) {
			int?[][] newDataset = new int?[Count][];
			int featureCount = _features.Count;
			for (int i = 0; i < Count; i++) {
				newDataset[i] = new int?[featureCount - featureIndices.Count()];
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

		public static int? ToDatasetValue(object o) {
			int? value = null;
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
