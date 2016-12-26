using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullInspector;
using MDS.Actions.DialogConditions;

public class CheckGameObject : ConditionedActionBase {

	[InspectorCategory("Condition")]
	[SerializeField, InspectorTooltip("Checar por meio de condition se um Object está ativado")]
	public GameObject IsThisGOActive;

	public override bool IsConditionSatisfied()
	{
		bool ret = false;


		if (IsThisGOActive.activeInHierarchy)
			ret = true;

		return ret;
		Debug.Log (ret);

	}
}
