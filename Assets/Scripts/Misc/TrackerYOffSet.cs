using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerYOffSet : MonoBehaviour {

	[SerializeField]
	private GameObject m_objReference;

	[SerializeField]
	private float m_offSet = -0.3f;

	[SerializeField]
	private Transform m_xOffSetReference;

	void Update()
	{
		transform.position = new Vector3 (m_xOffSetReference.transform.position.x, m_objReference.transform.position.y + m_offSet, m_objReference.transform.position.z);
	}

}
