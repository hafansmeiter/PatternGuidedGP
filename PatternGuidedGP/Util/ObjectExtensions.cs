using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Util {
	static class ObjectExtensions {
		public static double ToNumeric(this object obj) {
			if (obj.GetType() == typeof(int)) {
				return (int)obj;
			} else if (obj.GetType() == typeof(float)) {
				return (float)obj;
			} else if (obj.GetType() == typeof(double)) {
				return (double)obj;
			}
			return 0;
		}
	}
}
