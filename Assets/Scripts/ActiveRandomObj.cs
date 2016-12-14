using System.Collections;
using UnityEngine;

public class ActiveRandomObj : MonoBehaviour {


	[SerializeField]
	private GameObject[] m_obj;
	private GameObject m_currentObj;
	void OnEnable()
	{
		int random = Random.Range (0, m_obj.Length);
		m_currentObj = m_obj [random];
		m_currentObj.SetActive (true);
	}


	void OnDisable()
	{
		m_currentObj.SetActive (false);
	}

}
