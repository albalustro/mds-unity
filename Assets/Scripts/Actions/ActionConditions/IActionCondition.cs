using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MDS.Core.Interfaces;

namespace MDS.Actions.DialogConditions
{
    public interface IActionCondition
    {
        IAction[] Actions { get; set; }
        bool IsConditionSatisfied();
    }
}
