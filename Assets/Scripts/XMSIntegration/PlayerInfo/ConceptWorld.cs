using UnityEngine;
using System.Collections;

/// <summary>
/// Informações sobre o Mundo para composição do mapa de conceitos
/// Contém um array com todos os episodios do mundo em questão
/// </summary>
[System.Serializable]
public class ConceptWorld 
{
    public ConceptWorld()
    {

    }
	public ConceptEpisode[] episodes;
}
