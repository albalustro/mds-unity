using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullInspector;
using MDS.Gameplay.DragDrop;
using MDS.Gameplay.Selectable;
using UnityEngine;

namespace MDS.Actions.DialogConditions
{
    public class LastGenericValidationLabelActionCondition : ConditionedActionBase
    {
        [InspectorCategory("Condition")]
        [SerializeField]
        private ValidatableDropGroupArea _group;

        [InspectorCategory("Condition")]
        [SerializeField]
        private string[] _labels;

        public override bool IsConditionSatisfied()
        {

            if(!_labels.Contains(_group.GetGenericValidatedLabel()))
                return false;

            return true;
        }
    }
}
