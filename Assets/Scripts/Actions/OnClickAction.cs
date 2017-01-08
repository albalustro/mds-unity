using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MDS.Core;
using MDS.Core.Interfaces;
using FullInspector;

public class OnClickAction : MDSBehaviour {

    [InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
    public IAction[] m_onClickAction;


	void OnMouseUp()
	{
        ExecuteActions(m_onClickAction);
	}
}
