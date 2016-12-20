using System.Collections;
using System.Collections.Generic;
using MDS.Actions;
using UnityEngine;

public class MemorizeMe : BaseAction
{

    public static GameObject MemorizedGameObject;


    public bool selfTarget;

    [FullInspector.InspectorHideIf("selfTarget")]
    public GameObject GameObjectToBeMemorized;

	[HideInInspector]
	public static Vector2 m_transform;

    

    public override IEnumerator Execute()
    {
        yield return base.Execute();

		if (selfTarget) {
			GameObjectToBeMemorized = _corotineHolder.gameObject;
			m_transform = GameObjectToBeMemorized.transform.position;
		}

        MemorizedGameObject = GameObjectToBeMemorized;

    }
}