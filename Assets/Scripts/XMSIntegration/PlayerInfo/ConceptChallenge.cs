using UnityEngine;
using System.Collections;

/// <summary>
/// Informações sobre o Desafio para composição do mapa de conceitos
/// Contém o conceito obtido, data/hora de de inicio e data/hora de termino
/// </summary>
[System.Serializable]
public class ConceptChallenge
{
    public ConceptChallenge()
    {

    }
	public ConceptTypes concept;
	public string startDate;
	public string endDate;
}
