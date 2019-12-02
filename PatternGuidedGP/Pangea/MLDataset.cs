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

		private IList<IList<ulong>> _featureOrder = new List<IList<ulong>>();	// for chronological order

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
				IList<ulong> featureOrder = new List<ulong>(trace.Records.Count);
				foreach (var record in trace.Records) {
					object[] featureValues;
					if (!dataset._features.TryGetValue(record.NodeId, out featureValues)) {
						featureValues = new object[count];
						dataset._features.Add(record.NodeId, featureValues);
						dataset._featureData.Add(record.NodeId, 
							new MLDatasetFeature(record.NodeId, record.OperatorId));
					}
					featureValues[i] = record.Value;
					if (record.OperatorId > 0) {	// only use operators (no variables, constants)
						featureOrder.Add(record.NodeId);
					}
				}
				dataset._featureOrder.Add(featureOrder);
			}
			return dataset;
		}

		// takes first n and last n execution records including the operator id
		// in Krawiec - Pattern Guided GP, firstN and lastN is referred to as k
		// result dataset consists of firstN + lastN + 1 features (+1 for total execution records)
		public int?[][] ToRawChronologicalInputDataset(int firstN, int lastN, bool removeConstantFeatures = true) {
			int?[][] dataset = new int?[Count][];
			int featureCount = (firstN + lastN) * 2 + 1;

			for (int i = 0; i < Count; i++) {
				dataset[i] = new int?[featureCount];
				var opFeatures = _featureOrder[i];

				int k = 0, j = 0;	// added features
				for (j = 0; j < opFeatures.Count && j < firstN; j++) {
					var feature = opFeatures[j];
					var featureOp = _featureData[feature].OperatorId;
					var featureValue = _features[feature][i];
					dataset[i][k + j * 2] = featureOp;
					dataset[i][k + j * 2 + 1] = ToDatasetValue(featureValue);
				}
				k += j * 2;
				for (; k < firstN; k++) {   // fill up with null values
					dataset[i][k] = null;
				}

				for (j = 0; j < opFeatures.Count && j < lastN; j++) {
					var feature = opFeatures[opFeatures.Count - 1 - j];
					var featureOp = _featureData[feature].OperatorId;
					var featureValue = _features[feature][i];
					dataset[i][k + j * 2] = featureOp;
					dataset[i][k + j * 2 + 1] = ToDatasetValue(featureValue);
				}
				k += j * 2;
				for (; k < lastN; k++) {   // fill up with null values
					dataset[i][k] = null;
				}
				dataset[i][k] = opFeatures.Count;
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
			int rows = dataset.Length;
			int columns = dataset[0].Length;
			for (int i = 0; i < columns; i++) {
				bool isConstant = true;
				int value = 0;
				int j = 0;
				// find first not-null value
				for (j = 0; j < rows; j++) {
					if (dataset[j][i].HasValue) {
						value = dataset[j][i].Value;
						break;
					}
				}
				// compare value with rest of the array
				for (j += 1; j < rows; j++) {
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
			int rows = dataset.Length;
			int columns = dataset[0].Length;

			int?[][] newDataset = new int?[rows][];
			for (int i = 0; i < rows; i++) {
				newDataset[i] = new int?[columns - featureIndices.Count()];
				int featuresRemoved = 0;
				for (int j = 0; j < columns; j++) {
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
				} else if (o.GetType() == typeof(float)) {
					//value = (float)o;
				}
			}
			return value;
		}
	}
}
