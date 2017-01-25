using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// Informações sobre o Episódio para composição do mapa de conceitos
/// Contém um array com todos os desafios e o status de liberação do episódio
/// </summary>
[System.Serializable]
public class ConceptEpisode 
{
	public ConceptChallenge[] challenges;
	public EpisodeLiberationTypes liberationStatus;

	public bool CheckEpisodeComplete()
	{
		return challenges.All(c => c.concept == ConceptTypes.CONCEPT_GREEN);
	}

	public bool CheckChallengeComplete(int challengeIndex)
	{
		return challenges [challengeIndex].concept == ConceptTypes.CONCEPT_GREEN;
	}
}
