using UnityEngine;
using System.Collections;
using MDS.Gameplay.DragDrop;



public class DraggableFixedPosition : MDSBehaviour {

	[SerializeField]
	bool m_setPositions = false;

	private Draggable[] m_draggables;
	private int countIndex = 0;

	#if UNITY_EDITOR

	void OnValidate()
	{
		if (m_setPositions) {
			m_setPositions = false;
			SetPreferedPositionIndex ();
		}
	}

	#endif

	void SetPreferedPositionIndex()
	{
		m_draggables = GetComponentsInChildren<Draggable> ();
		for (int i = 0; i < m_draggables.Length; i++) {
			m_draggables [i].PreferredInitialIndex = countIndex;
			++countIndex;
		}

		countIndex = 0;

	}

}
