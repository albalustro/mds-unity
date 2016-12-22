using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullInspector;
using MDS.Gameplay.DragDrop;
using UnityEngine;

namespace MDS.Actions.DialogConditions
{
    public class SlotHasLabelCondition : ConditionedActionBase
    {
        [InspectorCategory("Condition")]
        [SerializeField]
        private DropGroupSlot _slot;

        [InspectorCategory("Condition")]
        [SerializeField]
        private string[] _labels;

        public override bool IsConditionSatisfied()
        {
            bool ret = false;
            
            if(_slot.draggableReference != null)
            {
                foreach(var label in _labels)
                {
                    if(_slot.draggableReference.Labels.Contains(label))
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
