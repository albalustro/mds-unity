using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MDS.Gameplay.Selectable;

public class TrackerXOffSet : MonoBehaviour {

	[SerializeField]
	private Transform m_xGlobalPosition;

	private Selectable _selectable;

	float _initialLocalX, _yInitialOffset;


	void Awake()
	{
		_selectable = GetComponent<Selectable>();
		_initialLocalX = transform.localPosition.x;
        _yInitialOffset = transform.localPosition.y;
	}

	void Update()
	{
		Vector3 pos = transform.position;
		if (_selectable.Selected)
		{
			pos.x = m_xGlobalPosition.position.x;
            pos.y = transform.parent.position.y - 0.3f + _yInitialOffset;
		}
		else
		{
			pos.x = transform.parent.position.x + _initialLocalX;
            pos.y = transform.parent.position.y + _yInitialOffset;

        }
		transform.position = pos;
	}

}
