using System;
using FullInspector;
using MDS.Core.Interfaces;
using UnityEngine;

namespace MDS.Actions.DialogConditions
{
    [Serializable]
    public abstract class ConditionedActionBase : IActionCondition
    {
        [InspectorCategory("Actions")]
        public IAction[] Actions { get; set; }
        public abstract bool IsConditionSatisfied();

    }
}
