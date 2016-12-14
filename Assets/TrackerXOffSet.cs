using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MDS.Gameplay.Selectable;

public class TrackerXOffSet : MonoBehaviour {

	private Selectable m_selectable;

	void Start()
	{
		m_selectable = GetComponent<Selectable> ();
	}

}
