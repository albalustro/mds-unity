using System.Collections;
using System;
using UnityEngine;
using FullInspector;
using MDS.Actions;
using MDS.Core.Interfaces;
using MDS.Gameplay.Selectable;

public class LockSelectableGroup : BaseAction {

	[SerializeField]
	private Transform m_groupTransform;

	[SerializeField, InspectorTooltip("Check this if you want to lock/unlock the selected group")]
	private bool m_lockGroup;


	public override IEnumerator Execute()
	{
		yield return base.Execute();

		foreach (Transform child in m_groupTransform) {
			child.GetComponent<Collider2D>().enabled = m_lockGroup;
		}

	}

}
