using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MDS.Core;
using MDS.Core.Interfaces;



public class OnClickAction : MDSBehaviour {


	public IAction[] m_onClickAction;


	IEnumerator OnMouseUp()
	{
		for(int i = 0; i < m_onClickAction.Length; i++)
		{
			if(m_onClickAction[i] == null)
			{
				LogError("Action não definida.");
				continue;
			}

			if(m_onClickAction[i].waitFinish)
				yield return StartCoroutine(m_onClickAction[i].Execute());
			else
				StartCoroutine(m_onClickAction[i].Execute());
		}
	}
}
