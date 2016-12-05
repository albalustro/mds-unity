using UnityEngine;
using System.Collections;

/// <summary>
/// Informações sobre o Episódio para composição do mapa de conceitos
/// Contém um array com todos os desafios e o status de liberação do episódio
/// </summary>
[System.Serializable]
public class ConceptEpisode 
{
	public ConceptChallenge[] challenges;
	public EpisodeLiberationTypes liberationStatus;
}
