using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MDS.Actions;
using MDS.Core.Interfaces;
using MDS.Core;

public class DroppedBag : MDSBehaviour {

	[SerializeField]
	private Transform m_YLimit;

	[SerializeField]
	private float m_fallSpeed;

	public IAction[] m_onFallAction;

	private bool m_isFalling;

	void OnEnable()
	{
		m_isFalling = false;
	}

	void Update()
	{
		if (!m_isFalling) {
			if (transform.position.y >= m_YLimit.position.y) {
				transform.position = new Vector2 (transform.position.x, transform.position.y - 0.5f * m_fallSpeed * Time.deltaTime);
			} else {
				m_isFalling = true;
				ExecuteActions (m_onFallAction);
			}
		}
	}

}
