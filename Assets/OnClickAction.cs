using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MDS.Core;
using MDS.Core.Interfaces;



public class OnClickAction : MDSBehaviour {


	public IAction[] m_onClickAction;


	void OnMouseUp()
	{
		for (int i = 0; i < m_onClickAction.Length; i++) {
			StartCoroutine(m_onClickAction[i].Execute());
		}
	}
}
