using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Operators {
	class TournamentSelector : ISelector {
		public int TournamentSize { get; set; }

		public TournamentSelector(int tournamentSize) {
			TournamentSize = tournamentSize;
		}

		public Individual Select(Population population) {
			Individual bestIndividual = population.GetRandom();
			for (int i = 0; i < TournamentSize - 1; i++) {
				Individual individual = population.GetRandom();
				if (individual.IsBetterThan(bestIndividual)) {
					bestIndividual = individual;
				}
			}
			return bestIndividual;
		}
	}
}
