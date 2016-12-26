using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullInspector;
using MDS.Actions.DialogConditions;

namespace MDS.Actions.DialogConditions
{
    public class CheckGameObject : ConditionedActionBase
    {
        [InspectorCategory("Condition")]
        [SerializeField, InspectorTooltip("Checar por meio de condition se um Object está ativado")]
        public GameObject IsThisGOActive;

        public override bool IsConditionSatisfied()
        {
            if(IsThisGOActive.activeInHierarchy)
                return true;
            return false;
        }
    }
}
