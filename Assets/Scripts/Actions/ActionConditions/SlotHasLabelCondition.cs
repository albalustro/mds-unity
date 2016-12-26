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
        [InspectorComment("Utilize um slot OU um grupo.")]
        [InspectorCategory("Condition")]
        [SerializeField, InspectorHideIf("HideSlot")]
        private DropGroupSlot _slot;
        private bool HideSlot { get { return _group != null; } }

        [InspectorCategory("Condition")]
        [SerializeField, InspectorHideIf("HideGroup")]
        private ValidatableDropGroupArea _group;
        private bool HideGroup { get { return _slot != null; } }

        [InspectorCategory("Condition")]
        [SerializeField]
        private string[] _labels;

        public override bool IsConditionSatisfied()
        {
            if(_slot != null)
            {
                if(_slot.draggableReference != null)
                {
                    if(_labels.Any(label => _slot.draggableReference.Labels.Contains(label)))
                        return true;
                    return false;
                }
            }

            if (_group!=null)
            {
                if(_labels.Any(label => _group.Validate(label)))
                    return true;
            }

            return false;

        }
    }
}
