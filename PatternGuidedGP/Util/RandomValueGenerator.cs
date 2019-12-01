using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Util {
	class RandomValueGenerator {
		public static RandomValueGenerator Instance {
			get {
				if (_store == null) {
					_store = new RandomValueGenerator();
				}
				return _store;
			}
		}
		private static RandomValueGenerator _store;
		private Random _random = new Random();

		public int GetInt(int maxValue) {
			return _random.Next(maxValue);
		}

		public double GetDouble() {
			return _random.NextDouble();
		}

		public double GetDouble(double maxValue) {
			return GetDouble() * maxValue;
		}

		public float GetFloat() {
			return (float)_random.NextDouble();
		}

		public float GetFloat(float maxValue) {
			return (float) GetDouble(maxValue);
		}

		public bool GetBool() {
			return GetDouble() >= 0.5;
		}

		public int GetInt(int minValue, int maxValue) {
			return _random.Next(maxValue - minValue) + minValue;
		}
	}
}
