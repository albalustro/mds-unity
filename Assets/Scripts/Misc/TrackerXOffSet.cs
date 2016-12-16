using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MDS.Gameplay.Selectable;

public class TrackerXOffSet : MonoBehaviour {

	[SerializeField]
	private Transform m_xGlobalPosition;

	private Selectable m_selectable;


	void Awake()
	{
		m_selectable = GetComponent<Selectable> ();
	}

	void Update()
	{
		float onSelectedPosition = m_xGlobalPosition.position.x - transform.position.x;
		Vector2 newPos = new Vector2 (onSelectedPosition, -0.3f);
		m_selectable._localPositionDisplacement.SelectedValue = newPos;
	}

}
