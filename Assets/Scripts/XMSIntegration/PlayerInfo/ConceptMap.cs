using UnityEngine;
using System.Collections;

/// <summary>
/// Informações sobre a Temporada para composição do mapa de conceitos
/// Contém um array com os 4 mundos
/// </summary>
[System.Serializable]
public class ConceptMap 
{
    public ConceptMap()
    {

    }
	public ConceptWorld[] worlds;
}
