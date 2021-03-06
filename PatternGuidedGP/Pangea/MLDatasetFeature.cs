﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Pangea {
	class MLDatasetFeature {
		public ulong NodeId { get; set; }
		public int OperatorId { get; set; }
		public Type Type { get; set; }

		public MLDatasetFeature(ulong nodeId, int operatorId, Type type) {
			NodeId = nodeId;
			OperatorId = operatorId;
			Type = type;
		}
	}
}
