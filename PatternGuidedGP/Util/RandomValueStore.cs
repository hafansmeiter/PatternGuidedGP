using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Util {
	class RandomValueStore {
		public static RandomValueStore Instance {
			get {
				if (_store == null) {
					_store = new RandomValueStore();
				}
				return _store;
			}
		}
		private static RandomValueStore _store;
		private Random _random = new Random();

		public int GetInt(int maxValue) {
			return _random.Next(maxValue);
		}

		public double GetDouble() {
			return _random.NextDouble();
		}
	}
}
