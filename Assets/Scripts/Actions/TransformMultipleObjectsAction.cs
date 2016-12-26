using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MDS.Actions;
using FullInspector;

public class TransformMultipleObjectsAction : BaseAction {

	[SerializeField, InspectorComment("Setar o transform de vários objetos ao mesmo tempo")]
	private GameObject[] m_multiTargets;

	[SerializeField]
	private Transform m_destination;

	public override IEnumerator Execute()
	{
		yield return base.Execute();

		for (int i = 0; i < m_multiTargets.Length; i++) {
			m_multiTargets [i].transform.position = m_destination.position;
		}


	}

}
