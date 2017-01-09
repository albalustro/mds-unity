using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullInspector;
using MDS.Core.Interfaces;

namespace MDS.Actions.DialogConditions
{
    public interface IActionCondition
    {
        [InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
        IAction[] Actions { get; set; }
        bool IsConditionSatisfied();
    }
}
