using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaloesMovment : MonoBehaviour {

	[SerializeField]
	private float m_speed;

	[SerializeField]
	private Transform m_startPosition, m_endPosition;

	void Start()
	{
		m_startPosition.SetParent (null);
		m_endPosition.SetParent (null);
	}

	void Update()
	{
		foreach (Transform child in transform) {
			child.position = new Vector3 (child.transform.position.x + 0.1f * m_speed * Time.deltaTime, child.position.y, transform.position.z);
			if (child.position.x >= m_endPosition.position.x) {
				child.position = new Vector3 (m_startPosition.position.x, child.position.y, transform.position.z);
			}
		}
	}

}
