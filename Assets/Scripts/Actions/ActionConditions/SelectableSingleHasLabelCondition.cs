using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullInspector;
using MDS.Gameplay.Selectable;
using UnityEngine;

namespace MDS.Actions.DialogConditions
{
    public class SelectableSingleHasLabelCondition : ConditionedActionBase
    {
        [InspectorCategory("Condition")]
        [SerializeField]
        private SelectableGroup _selectableGroup;

        [InspectorCategory("Condition")]
        [SerializeField]
        private string[] _labels;


        public override bool IsConditionSatisfied()
        {
            bool ret = false;

            Selectable s = _selectableGroup.GetSelectedSingle();

            if(s != null)
            {
                foreach(var label in _labels)
                {
                    if(s.Labels.Contains(label))
                    {
                        ret = true;
                        break;
                    }
                }
            }

            return ret;


        }
    }
}
