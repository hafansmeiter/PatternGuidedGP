﻿using System;
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

		public bool GetBool() {
			return GetDouble() >= 0.5;
		}
	}
}