using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Se precisar fazer algum gerenciamento ou preset no mapa, deve ser feito nesse componente
/// </summary>
public class MapController : MonoBehaviour {

	public AudioClip mapTheme;

	// Use this for initialization
	void Start () {
		AudioController.Instance.PlayTheme (mapTheme);
	}
}
